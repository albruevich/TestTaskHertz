DOTNET_BACKEND ?= $(shell if [ -x /usr/local/opt/dotnet@9/libexec/dotnet ]; then echo /usr/local/opt/dotnet@9/libexec/dotnet; else echo dotnet; fi)
DOTNET_MOBILE ?= dotnet

.PHONY: b f f-logs api restore build-api

b api:
	$(DOTNET_BACKEND) run --no-restore --project src/TestTaskHertz.Api/TestTaskHertz.Api.csproj

f:
	@set -e; \
	UDID=$$(xcrun simctl list devices booted | awk -F'[()]' '/Booted/ {print $$2; exit}'); \
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
	APP="src/TestTaskHertz.Mobile/bin/Debug/net9.0-ios/iossimulator-arm64/TestTaskHertz.Mobile.app"; \
	$(DOTNET_MOBILE) build src/TestTaskHertz.Mobile/TestTaskHertz.Mobile.csproj -f net9.0-ios -r iossimulator-arm64 --tl:off -v:q; \
	xcrun simctl terminate "$$UDID" com.testtaskhertz.mobile >/dev/null 2>&1 || true; \
	xcrun simctl install "$$UDID" "$$APP"; \
	xcrun simctl launch "$$UDID" com.testtaskhertz.mobile

f-logs:
	xcrun simctl spawn booted log stream --style compact --predicate 'process == "TestTaskHertz.Mobile"'

restore:
	$(DOTNET_MOBILE) restore TestTaskHertz.sln

build-api:
	$(DOTNET_BACKEND) build src/TestTaskHertz.Api/TestTaskHertz.Api.csproj
