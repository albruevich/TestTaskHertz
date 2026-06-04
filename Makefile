.PHONY: b f f-logs api restore build-api

b api:
	dotnet run --project src/TestTaskHertz.Api/TestTaskHertz.Api.csproj

f:
	@UDID=$$(xcrun simctl list devices booted | awk -F'[()]' '/Booted/ {print $$2; exit}'); \
	if [ -z "$$UDID" ]; then \
		open -a Simulator; \
		for i in $$(seq 1 30); do \
			UDID=$$(xcrun simctl list devices booted | awk -F'[()]' '/Booted/ {print $$2; exit}'); \
			if [ -n "$$UDID" ]; then break; fi; \
			sleep 1; \
		done; \
	fi; \
	if [ -z "$$UDID" ]; then \
		echo "No booted iOS Simulator found."; \
		exit 1; \
	fi; \
	APP="src/TestTaskHertz.Mobile/bin/Debug/net10.0-ios/iossimulator-arm64/TestTaskHertz.Mobile.app"; \
	dotnet build src/TestTaskHertz.Mobile/TestTaskHertz.Mobile.csproj -f net10.0-ios -r iossimulator-arm64 --tl:off -v:q; \
	xcrun simctl terminate "$$UDID" com.testtaskhertz.mobile >/dev/null 2>&1 || true; \
	xcrun simctl install "$$UDID" "$$APP"; \
	xcrun simctl launch "$$UDID" com.testtaskhertz.mobile

f-logs:
	xcrun simctl spawn booted log stream --style compact --predicate 'process == "TestTaskHertz.Mobile"'

restore:
	dotnet restore TestTaskHertz.sln

build-api:
	dotnet build src/TestTaskHertz.Api/TestTaskHertz.Api.csproj
