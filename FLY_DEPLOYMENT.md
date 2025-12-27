# Arab Engineers Forum - Fly.io Deployment

This file contains instructions for deploying to Fly.io.

## Prerequisites
- Fly.io account (sign up at https://fly.io)
- flyctl CLI installed

## Deployment Steps

### 1. Install Fly.io CLI
```powershell
iwr https://fly.io/install.ps1 -useb | iex
```

### 2. Login to Fly.io
```bash
fly auth login
```

### 3. Create PostgreSQL Database
```bash
fly postgres create --name arab-forum-db --region ams
```
Save the connection string that appears!

### 4. Attach Database to App
```bash
fly postgres attach arab-forum-db --app arab-engineers-forum
```

### 5. Deploy Application
```bash
fly deploy
```

### 6. Set Connection String
The DATABASE_URL will be automatically set. Update appsettings.json to read from environment:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=...;Database=...;Username=...;Password=..."
}
```

## Monitoring
- View logs: `fly logs`
- Open app: `fly open`
- Check status: `fly status`

## Free Tier Limits
- 3 shared-cpu-1x 256mb VMs
- 3GB persistent volume storage
- 160GB outbound data transfer
