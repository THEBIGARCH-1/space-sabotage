# Deploy Space Sabotage as Public Website

## DEPLOYMENT GUIDE - Complete Instructions

---

## Part 1: WebGL Build for Browser

### Step 1: Install WebGL Support
```
1. Open Unity Hub
2. Click gear icon on your project → Add Modules
3. Search and install "WebGL Build Support"
4. Restart Unity
```

### Step 2: Configure Build Settings
```
1. File → Build Settings
2. Click "Add Open Scenes" (adds current scene)
3. Platform: Select WebGL
4. Click "Switch Platform"
5. Player Settings:
   - Product Name: "Space Sabotage"
   - Resolution: 1920x1080
   - Compression: Brotli
```

### Step 3: Build WebGL
```
1. File → Build
2. Create folder: "Build_WebGL"
3. Click Build (wait 15-30 minutes)
4. Output files in Build_WebGL/
```

### Step 4: Test Locally
```bash
# Install Python HTTP server
python -m http.server 8000

# Navigate to: http://localhost:8000/Build_WebGL/
```

---

## Part 2: Backend Server Setup

### Option A: Netcode Cloud (Easiest - Recommended)

```
1. Go to https://cloud.netcode.io/
2. Sign in with Unity account
3. Create project → Get API key
4. Paste in Unity: Window → Netcode → Cloud Transport
5. Select region closest to players
6. Deploy automatically
```

### Option B: Self-Hosted Server

**Using Node.js:**

Create `server.js`:
```javascript
const express = require('express');
const http = require('http');
const socketIo = require('socket.io');

const app = express();
const server = http.createServer(app);
const io = socketIo(server, { cors: { origin: "*" } });
const PORT = process.env.PORT || 3000;

io.on('connection', (socket) => {
  console.log('Player connected:', socket.id);
  socket.on('playerMove', (data) => {
    socket.broadcast.emit('playerMove', data);
  });
  socket.on('disconnect', () => {
    console.log('Player disconnected');
  });
});

server.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
```

Setup:
```bash
mkdir server
cd server
npm init -y
npm install express http socket.io
node server.js
```

---

## Part 3: Deploy to Web Hosting

### OPTION 1: Itch.io (BEST FOR GAMES - RECOMMENDED)

**Simplest option - 3 steps:**

```
1. Create account at https://itch.io/
2. Click Dashboard → Create new project
3. Upload Build_WebGL folder as "WebGL" build
4. Your game link: yourusername.itch.io/space-sabotage
5. Share with friends!
```

**Done! Your game is live.**

---

### OPTION 2: GitHub Pages (Free)

```bash
# Copy WebGL build to docs folder
mkdir docs
cp -r Build_WebGL/* docs/

# Push to GitHub
git add docs/
git commit -m "Add WebGL build"
git push origin main

# GitHub Settings → Pages
# Source: Deploy from branch → main /docs folder

# Your game: https://THEBIGARCH-1.github.io/space-sabotage/
```

---

### OPTION 3: Netlify (Easy & Free)

```bash
# Install Netlify CLI
npm install -g netlify-cli

# Login
netlify login

# Deploy
netlify deploy --prod --dir=Build_WebGL

# Your game: space-sabotage.netlify.app
```

---

### OPTION 4: AWS S3 + CloudFront (Professional)

```bash
# Create S3 bucket
aws s3 mb s3://space-sabotage-game

# Upload build
aws s3 sync Build_WebGL/ s3://space-sabotage-game --delete

# Enable public access in AWS console
# Create CloudFront distribution
# Your game: yourdomain.com
```

---

## Part 4: Connect Game to Server

Update `PlayerNetworkSync.cs`:

```csharp
private void Start()
{
    // Use environment variable or hardcode
    string serverIP = System.Environment.GetEnvironmentVariable("GAME_SERVER") ?? "game-server.com";
    ushort port = 7777;
    
    NetworkManager.Singleton.GetComponent<UnityTransport>()
        .SetConnectionData(serverIP, port);
}
```

---

## Part 5: Custom Domain (Optional)

### Buy Domain
- Godaddy.com
- Namecheap.com
- Domain.com

### Connect to Netlify
```
1. Netlify Dashboard → Domain Settings
2. Add custom domain: yourgame.com
3. Update DNS records to Netlify nameservers
4. Wait 24-48 hours
```

### Connect to GitHub Pages
```
1. Buy domain
2. Update DNS A record to: 185.199.108.153
3. GitHub Settings → Pages → Add custom domain
```

---

## QUICK START (5 Minutes)

**Fastest way to go public:**

```bash
# 1. Build WebGL
# File → Build Settings → WebGL → Build

# 2. Upload to Itch.io
# Sign up → Create project → Upload Build_WebGL

# 3. Share link
# yourusername.itch.io/space-sabotage
```

**That's it! Your game is online.**

---

## Deployment Checklist

- [ ] WebGL build created and tested
- [ ] Server deployed (Netcode Cloud or self-hosted)
- [ ] Game uploaded to hosting (Itch.io recommended)
- [ ] Tested multiplayer with 2+ players
- [ ] Custom domain set up (optional)
- [ ] HTTPS enabled (free with Let's Encrypt)
- [ ] Analytics added (Firebase)
- [ ] Error logging set up (Sentry)

---

## Monthly Cost Breakdown

| Service | Cost | Purpose |
|---------|------|----------|
| Itch.io | Free | Host game |
| Netcode Cloud | Free (up to 100 concurrent) | Multiplayer backend |
| Custom Domain | $10-15 | yourgame.com |
| **TOTAL** | **$10-15/month** | **Everything** |

---

## Scaling When Popular

If game gets 1000+ players:

1. **Load Balancer**: AWS ELB ($20/month)
2. **Multiple Servers**: 3x servers ($30/month)
3. **Database**: PostgreSQL or Firebase ($50/month)
4. **CDN**: Cloudflare ($200/month)
5. **Total**: ~$300/month for 10,000 concurrent players

---

## Support Resources

- Itch.io Help: https://itch.io/docs
- Netlify Docs: https://docs.netlify.com/
- GitHub Pages: https://pages.github.com/
- Netcode Docs: https://docs-multiplayer.unity3d.com/
- AWS Guide: https://aws.amazon.com/getting-started/

---

## You're Ready to Launch! 🚀

Choose Itch.io for easiest deployment. Your game will be playable in 10 minutes.
