
// RUN ME using "node contract_test.js"


const fs = require("fs");
const http = require("http");

// URLs and file paths
const url = "http://localhost:5001/swagger/v1/swagger.json";
const expectedFilePath = "test/PaymentGateway.Api.Tests/contract/expected_contract.json";

// Helper to sort JSON keys
function normalizeJson(json) {
  return JSON.stringify(JSON.parse(json), Object.keys(JSON.parse(json)).sort(), 2);
}

// Fetch data from URL
function fetchApiDocs(url, callback) {
  http.get(url, (res) => {
    let data = "";
    res.on("data", (chunk) => (data += chunk));
    res.on("end", () => callback(null, data));
  }).on("error", (err) => callback(err));
}

// Compare the actual and expected JSON
fetchApiDocs(url, (err, actualData) => {
  if (err) {
    console.error(`❌ Error fetching API docs: ${err.message}`);
    return;
  }

  // Check if the expected file exists
  if (!fs.existsSync(expectedFilePath)) {
    console.error(`❌ Expected file not found: ${expectedFilePath}`);
    return;
  }

  const expectedData = fs.readFileSync(expectedFilePath, "utf8");

  // Normalize JSON
  const normalizedActual = normalizeJson(actualData);
  const normalizedExpected = normalizeJson(expectedData);

  // Compare normalized JSON
  if (normalizedActual === normalizedExpected) {
    console.log("✅ API documentation matches the expected JSON.");
  } else {
    console.log("❌ API documentation does not match the expected JSON.");
    console.log("=== Differences ===");
    console.log(normalizedActual);
    console.log(normalizedExpected);
  }
});
