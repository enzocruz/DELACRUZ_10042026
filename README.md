# File Processing API

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) – to build, run and test locally
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) – to run in a container
- Git – to clone the repository

Optional:
- OpenSSL – to generate an API key (`openssl rand -base64 32`) you can use the any key stirng 
- curl or [Postman](https://www.postman.com/downloads/) or swagger 

## Configuration 

X-API-KEY is required at every request.

Generate key:
```bash
openssl rand -base64 32
```
or use any string but preferably use a secure key.

**Running locally:** copy the appsettings.exmaplye.json and put your key in `ApiKey`:
```bash
cp FileProcessing.Api/appsettings.example.json FileProcessing.Api/appsettings.json
```

**Running with Docker:** copy the .example.env and rename it to .env and add your key  `APIKEY`:
```bash
cp .example.env .env
```

## Build

```bash
dotnet build
```

## Run locally

```bash
dotnet run --project FileProcessing.Api
```

The API runs on `http://localhost:5099`.
Open `http://localhost:5099/swagger` to try it in the browser.

Alternative way and my prefered way of running it locally. 
```bash
  dotnet watch
```

## Run with Docker

```bash
docker compose up --build 
```
or for detached
```bash
docker compose up -d --build  
```

The API runs on `http://localhost:8085`.
Open `http://localhost:8085/swagger`.

To stop it:
```bash
docker compose down
```

Without compose (But much prefered for compose)
```bash
docker build -t fileprocessing-api .
docker run --rm -p 8085:8085 --env-file .env fileprocessing-api
```

## Test 

```bash 
dotnet test 
```
There are 12 unit tests:

- **File processing:** reject json files that ar malformed (missing properties,unknown properties), filter by age over 18
- **Report:** check if files are being correctly recorded.
- **Upload endpoint:** returns a 500 when processing fails unexpectedly.

## API Reference

Base URL: `http://localhost:5099` (local) or `http://localhost:8085` (Docker).
All endpoints need the `X-API-Key` header.

### Upload a file

```http
POST /api/files/upload
```

Content type: `multipart/form-data`

| Parameter | Type   | Description |
| :-------- | :----- | :---------- |
| `file`    | `file` | **Required.** A `.json` file with an array of customers |

This are the filed for the customer, anything missing or unknown fields is rejected with a 400 .

```json
{
  "CustomerName": "John Smith",
  "CustomerId": "CUST-10001",
  "CustomerEmail": "john.smith@example.com",
  "CustomerPhone": "+63 917 123 4567",
  "CustomerAge": 20
}
```

Only customers aged 18 or over are kept.

```bash
curl -X POST http://localhost:5099/api/files/upload \
  -H "X-API-Key: $APIKEY" \
  -F "file=@samples/customer.json;type=application/json"
```

Response `200`:
```json
{
  "fileName": "customer.json",
  "recordsProcessed": 3,
  "recordsAccepted": 2,
  "customers": [
    {
      "customerName": "John Smith",
      "customerId": "CUST-10001",
      "customerEmail": "john.smith@example.com",
      "customerPhone": "+63 917 123 4567",
      "customerAge": 20
    },
    {
      "customerName": "John Doe",
      "customerId": "CUST-10003",
      "customerEmail": "john.doe@example.com",
      "customerPhone": "+63 917 123 4569",
      "customerAge": 25
    }
  ],
  "recordsRejected": 1
}
```

### Get the report

```http
GET /api/reports
```

No parameters. Returns every file that is processed.

```bash
curl http://localhost:5099/api/reports -H "X-API-Key: $APIKEY"
```

Response `200`:
```json
{
  "totalFiles": 1,
  "fileRecords": [
    {
      "fileName": "customer.json",
      "totalRecordCount": 3,
      "acceptedRecordCount": 2,
      "fileSize": 549,
      "processingStart": "2026-10-04T16:52:21.415795Z",
      "processingEnd": "2026-10-04T16:52:21.432353Z",
      "rejectedRecordCount": 1,
      "proccessingTimeMS": 16.558
    }
  ]
}
```

## Authentication

Every API request needs the key in the `X-API-Key` header.

| Request | Response |
| :--- | :--- |
| No `X-API-Key` header | `401` `{ "error": "API key is required." }` |
| Wrong key | `401` `{ "error": "Invalid API key." }` |
| Correct key | The request goes through |


**In Swagger:** click **Authorize** at the top, paste your key, then click **Authorize** .

**With curl:**
```bash
curl http://localhost:5099/api/reports -H "X-API-Key: $APIKEY"
```

## File tracking

Every successfully file processed can be view to  `GET /api/reports`.

For each file, the report keeps:

| Field | Meaning |
| :--- | :--- |
| `fileName` | Name of the Uploaded file |
| `totalRecordCount` | Number of customers in the file |
| `acceptedRecordCount` | Customers kept are over 18  |
| `rejectedRecordCount` | Customers filtered out (Value is calculated base on totalRecordCount-acceptedRecordCount ) |
| `fileSize` | File size in bytes |
| `processingStart` / `processingEnd` | Process start and process end date |
| `proccessingTimeMS` | How long the file is proceess in MS|

`totalFiles` number of files processed 

**How it works:**
- The upload endpoint records the processing, then  creates the FileRecord for `FileReport` once a file is processed.
- `FileReport` is registered as a singleton to serve as in memory list of records using `ConcurrentBag` for threadsafety.
- Only files that are processed successfully are recorded. 

**Logging:** each upload is also written to the console log, for example:
```
info: Processed customer.json: 2 accepted in 16.558 ms
warn: Rejected invalid.json: invalid JSON
```
## Samples and Postman

The `samples/` folder has test files for the upload endpoint:

| File | Result |
| :--- | :--- |
| `customer.json` | `200`: 3 customers processed, 2 accepted 1 rejected age is 17 |
| `invalid.json` | `400`: records with missing fields |
| `invalid-unmap.json` | `400`: a record with an extra field (`FirstName`) |

A Postman collection is also included (`File Processing API.postman_collection.json`). It uses an `APIKEY`and `baseURL`  variable.

