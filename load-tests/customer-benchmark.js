
import http from 'k6/http';
import { check } from 'k6';

export const options = {
  scenarios: {
    customer_read_load: {
      executor: 'constant-vus',
      vus: 20,
      duration: '1m',
    },
  },

  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<500', 'p(99)<1000'],
  },
};

export default function () {
  const customerId = "ffff15d2-f691-cfba-4fd8-49777ccfa343";

  if (!customerId) {
    throw new Error('Please set CUSTOMER_ID');
  }

  const url = `http://localhost:5226/api/customers/${customerId}`;

  const response = http.get(url, {
    tags: { endpoint: 'customer_get' },
  });

  check(response, {
    'status is 200': (r) => r.status === 200,
  });
}
