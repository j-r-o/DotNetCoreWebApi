#!/bin/bash
while true
do
	current_date_time="`date +%Y%m%d%H%M%S`";
	OUTPUT="$(curl -s 'https://api.ofx.com/PublicSite.ApiService/OFX/spotrate/Individual/AUD/EUR/1000?format=json' | jq -r '.InterbankRate')"

	#STEP1=${OUTPUT} | sed 's/*\{\(.*\)\}.*$/\1/'
	#echo 'first url, second url, {third url' | sed 's/.*\{//'
	#echo ${STEP1} | sed 's/(.*)/\1/'

	echo '{date="'${current_date_time}'", AudToEur="'${OUTPUT:0:6}'"},' >> rates.json
	echo '{date="'${current_date_time}'", AudToEur="'${OUTPUT:0:6}'"},'
	sleep 1s
done