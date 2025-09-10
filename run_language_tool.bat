docker pull erikvl87/languagetool
docker run -d --name lt -p 8010:8010 -e Java_Xms=512m -e Java_Xmx=1536m erikvl87/languagetool