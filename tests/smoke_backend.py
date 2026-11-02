"""Integration smoke test against the compiled real HTTP host; no production resources."""
import json, os, subprocess, time, urllib.request, urllib.error
from pathlib import Path
root = Path(__file__).resolve().parents[1]
base = "http://127.0.0.1:5197"
env = dict(os.environ, ASPNETCORE_URLS=base, ASPNETCORE_ENVIRONMENT="Production")
process = subprocess.Popen(["dotnet", str(root / "artifacts/web/Itsm.Web.dll")], cwd=root / "artifacts/web", env=env)
def request(path):
    try:
        response = urllib.request.urlopen(base + path, timeout=2)
    except urllib.error.HTTPError as error:
        response = error
    return response.status, response.headers, response.read()
try:
    for _ in range(60):
        if process.poll() is not None: raise RuntimeError("Host terminated")
        try:
            if request('/health/live')[0] == 200: break
        except OSError: pass
        time.sleep(.25)
    else: raise RuntimeError("Host did not start")
    status, headers, data = request('/api/v1/system')
    assert status == 200 and headers['Cache-Control'] == 'no-store'
    system = json.loads(data)
    assert system['stage'] == 'foundation'
    assert not system['authenticationEnabled'] and not system['persistenceEnabled'] and not system['acceptingTickets']
    assert request('/health/ready')[0] == 503
    assert request('/api/v1/nonexistent')[0] == 404
    assert request('/health/nonexistent')[0] == 404
    status, headers, body = request('/')
    assert status == 200 and b'root' in body
    assert "frame-ancestors 'none'" in headers['Content-Security-Policy']
    print('PASS: HTTP host, readiness, public configuration, API 404, UI shell and security headers')
finally:
    process.terminate()
    try: process.wait(timeout=5)
    except subprocess.TimeoutExpired: process.kill(); process.wait()
