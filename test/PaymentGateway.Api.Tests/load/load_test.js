import http from 'k6/http';
import { check, sleep } from 'k6';

// Configuration for load testing
export const options = {
    stages: [
        { duration: '30s', target: 10 },  // Ramp up to 10 users in 30 seconds
        { duration: '1m', target: 10 },   // Stay at 10 users for 1 minute
        { duration: '30s', target: 0 },   // Ramp down to 0 users in 30 seconds
    ],
};

export default function () {
    // Define the endpoint to test
    const url = 'http://localhost:5001/api/payments';

    // Create a sample payload for testing
    const payload = JSON.stringify({
        "cardNumber": "2222405343248877",
        "expiryMonth": "04",
        "expiryYear": "2025",
        "currency": "GBP",
        "amount": 100,
        "cvv": "123"
    });

    // Define headers
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    // Send a POST request
    const response = http.post(url, payload, params);

    // Validate response with checks
    check(response, {
        'status is 200': (r) => r.status === 200,
        'response contains id': (r) => JSON.parse(r.body).hasOwnProperty("id"),
        'payment should be authorized': (r) => JSON.parse(r.body).status === "Authorized",
        'response time is below 500ms': (r) => r.timings.duration < 500,
    });

    // Simulate a small wait between requests
    sleep(1);
}
