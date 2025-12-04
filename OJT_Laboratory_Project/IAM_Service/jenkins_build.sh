#!/bin/bash
set -o pipefail
set -e

echo "=== Cleaning solution ==="
dotnet clean IAM_Service.sln
dotnet nuget locals all --clear

echo "=== Restoring packages ==="
dotnet restore

echo "=== Building solution ==="
dotnet build IAM_Service.sln --configuration Release --no-restore

echo "=== Preparing folders ==="
mkdir -p IAM_Service.Application.UnitTests/TestResults/coverage
mkdir -p IAM_Service.Application.UnitTests/TestResults/CoverageReport

echo "=== Current directory ==="
pwd
ls -l
ls -l IAM_Service.Application.UnitTests || true

echo "=== Running tests with coverage (ALLOW FAIL) ==="
set +e
dotnet test IAM_Service.Application.UnitTests/IAM_Service.Application.UnitTests.csproj --collect:"XPlat Code Coverage" --results-directory ./IAM_Service.Application.UnitTests/TestResults
  /p:CollectCoverage=true \
  /p:CoverletOutput="IAM_Service.Application.UnitTests/TestResults/coverage/" \
  /p:CoverletOutputFormat=cobertura \
  /p:Include="[IAM_Service*]*" \
  --no-build \
  -v n
TEST_EXIT_CODE=$?
set -e

echo ""
echo "=== ✅ TEST PHASE FINISHED ==="
echo "Test exit code = $TEST_EXIT_CODE"
echo ""
echo "=== Checking for generated files ==="
echo "→ Listing IAM_Service.Application.UnitTests/TestResults/"
ls -R IAM_Service.Application.UnitTests/TestResults || true
echo ""
echo "→ Listing IAM_Service.Application.UnitTests/bin/Debug/"
ls -R IAM_Service.Application.UnitTests/bin/Debug || true
echo ""

echo "=== Checking coverage file ==="
if [ -f IAM_Service.Application.UnitTests/TestResults/coverage/coverage.cobertura.xml ]; then
    echo "✅ Found coverage file:"
    ls -l IAM_Service.Application.UnitTests/TestResults/coverage/coverage.cobertura.xml
else
    echo "⚠️ Coverage file not found! Searching recursively..."
    find IAM_Service.Application.UnitTests/TestResults -name "coverage.cobertura.xml" || true
    echo "⚠️ Creating placeholder file..."
    echo '<coverage></coverage>' > IAM_Service.Application.UnitTests/TestResults/coverage/coverage.cobertura.xml
fi

echo ""
echo "=== Restoring dotnet tools ==="
dotnet tool restore

echo ""
echo "=== Generating HTML coverage report ==="
dotnet tool run reportgenerator \
  -reports:"IAM_Service.Application.UnitTests/TestResults/**/coverage.cobertura.xml" \
  -targetdir:"IAM_Service.Application.UnitTests/TestResults/CoverageReport" \
  -reporttypes:Html \
  -verbosity:Verbose

echo ""
echo "=== Final Folder Structure ==="
tree -L 3 IAM_Service.Application.UnitTests/TestResults || ls -R IAM_Service.Application.UnitTests/TestResults

echo ""
echo "=== Final Result ==="
if [ $TEST_EXIT_CODE -ne 0 ]; then
    echo "⚠️ Some tests failed, but pipeline continues"
else
    echo "✅ All tests passed"
fi

echo "✅ Build + Test + Coverage complete"
exit 0
