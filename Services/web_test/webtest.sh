#!/bin/bash
url=$1
appName=$2
status_code=$(curl --write-out %{http_code} --silent --output /dev/null $url)

if [[ "$status_code" -eq 200 ]] ; then
  echo "$2 Web App is up and running here: $url."
else
  echo "$url does not appear to be available." && exit 1
fi