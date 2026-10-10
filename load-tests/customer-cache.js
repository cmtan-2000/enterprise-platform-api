import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 100,
  iterations: 100,
};

export default function () {
  const customerId = "ffff15d2-f691-cfba-4fd8-49777ccfa343";
  const url = `http://localhost:5226/api/customers/${customerId}`;

  const response = http.get(url);

  check(response, {
    'status is 200': (r) => r.status === 200,
  });
}