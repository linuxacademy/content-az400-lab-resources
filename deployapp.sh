echo "Starting deployment script run" && sudo npm install && sudo pm2 start -f app.js && sudo pm2 startup systemd &&  echo "End of script run"
