# ScheduleMe API Documentation

## Overview

ScheduleMe is an API for scheduling and managing jobs. The API allows you to schedule new jobs, list existing jobs, pause jobs, and stop jobs.

## API Endpoints

### Schedule a Job

**POST** `/Job/schedule`

**Tags:** Job

**Request Body:**

- **Content Types:**
  - `application/json`
  - `text/json`
  - `application/*+json`

- **Schema:**
  - `$ref: #/components/schemas/JobRequest`

**Sample Request:**

```json
{
  "name": "exampleJob",
  "cronExpression": "0/5 * * * * ?",
  "url": "https://example.com/api/task",
  "headers": {
    "Authorization": "Bearer token"
  },
  "body": "{\"key\":\"value\"}"
}
```

**Responses:**

- **200 OK:** Job scheduled successfully.

### List Jobs

**GET** `/Job/list`

**Tags:** Job

**Responses:**

- **200 OK:** List of jobs retrieved successfully.

### Pause a Job

**POST** `/Job/pause/{jobName}`

**Tags:** Job

**Parameters:**

- **Path Parameters:**
  - `jobName` (string, required): The name of the job to pause.

**Responses:**

- **200 OK:** Job paused successfully.

### Stop a Job

**POST** `/Job/stop/{jobName}`

**Tags:** Job

**Parameters:**

- **Path Parameters:**
  - `jobName` (string, required): The name of the job to stop.

**Responses:**

- **200 OK:** Job stopped successfully.

## Components

### Schemas

#### JobRequest

- **Type:** object
- **Properties:**
  - `name` (string, nullable): The name of the job.
  - `cronExpression` (string, nullable): The cron expression for the job schedule.
  - `url` (string, nullable): The URL to be called by the job.
  - `headers` (object, nullable): The headers to be included in the job request.
    - **Additional Properties:**
      - `type`: string
  - `body` (string, nullable): The body of the job request.
- **Additional Properties:** false