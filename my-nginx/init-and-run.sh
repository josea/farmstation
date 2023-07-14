trap exit TERM;

envsubst < /etc/nginx/conf.d/app.conf.template > /etc/nginx/conf.d/app.conf 

path=/etc/letsencrypt

if [ ! -e $path/nginx-phase1.pid ]; then 
	echo 'Initiating nginx with dummy certificates'
	nginx -g 'daemon off;' &	

	sleep 10s;	
	
	echo 'completed1' > $path/nginx-phase1.pid	
else
	echo 'Starting nginx'
	nginx -g 'daemon off;' &
	echo 'Started nginx'
fi

if [ ! -e $path/nginx-phase2.pid ]; then 	
	echo 'Entering phase2'
	while [ ! -e $path/certbot-phase2.pid ]; do echo 'Waiting for certificates to be created'; sleep 2s; done; 		


	echo 'Reloading new certificates!'
	nginx -s reload;
	echo 'completed2' > $path/nginx-phase2.pid
fi

echo 'Initiating periodic reload setings loop'

while :; do sleep 6h & wait ${!}; nginx -s reload; echo 'nginx reloaded'; done 

#nginx -g 'daemon off;' 


