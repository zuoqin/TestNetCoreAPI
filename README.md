# Bearer Token
## Creating Bearer Token
POST http://localhost:5000/token
Content-Type: application/x-www-form-urlencoded

username=TEST&password=TEST123


{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJURVNUIiwianRpIjoiMDMwYTVjY2MtOWJlYi00MTA1LTk0ZmItYmU5YTA3YWU4ZjQ1IiwiaWF0IjoxNDcwNTMxOTU3LCJuYmYiOjE0NzA1MzE5NTYsImV4cCI6MTQ3MDUzMjI1NiwiaXNzIjoiRXhhbXBsZUlzc3VlciIsImF1ZCI6IkV4YW1wbGVBdWRpZW5jZSJ9.01LB2fAoiFlVRz2BA55XJtJjgH0qLEC9lUvNiSS5Fek",
  "expires_in": 300
}

## Using Token
GET http://localhost:5000/api/values
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJURVNUIiwianRpIjoiMDMwYTVjY2MtOWJlYi00MTA1LTk0ZmItYmU5YTA3YWU4ZjQ1IiwiaWF0IjoxNDcwNTMxOTU3LCJuYmYiOjE0NzA1MzE5NTYsImV4cCI6MTQ3MDUzMjI1NiwiaXNzIjoiRXhhbXBsZUlzc3VlciIsImF1ZCI6IkV4YW1wbGVBdWRpZW5jZSJ9.01LB2fAoiFlVRz2BA55XJtJjgH0qLEC9lUvNiSS5Fek

["value1","value2"]