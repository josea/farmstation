trap exit TERM;

path=/etc/letsencrypt
rsa_key_size=4096
domains=farmstation.zapto.org

# copies the defaults that are in the docker image.
cp -nf /letsencrypt.defaults/* /etc/letsencrypt/

certpath="/etc/letsencrypt/live/$domains"

#	mkdir -p $certpath
#	openssl req -x509 -nodes -newkey rsa:$rsa_key_size -days 1\
#    -keyout $certpath/privkey.pem \
#    -out $certpath/fullchain.pem \
#   -subj /CN=localhost 

if [ ! -e $path/certbot-phase1.pid ]; then 
	echo 'First time -> generating dummy certificates!'

	mkdir -p $certpath

	openssl req -x509 -nodes -newkey rsa:$rsa_key_size -days 1\
    -keyout $certpath/privkey.pem \
    -out $certpath/fullchain.pem \
    -subj /CN=localhost 

	echo 'completed1' > $path/certbot-phase1.pid
fi 

if [ ! -e $path/certbot-phase2.pid ]; then 
	# waits for nginx to start
	while [ ! -e $path/nginx-phase1.pid ]; do echo 'Waiting for nginx to start'; sleep 2s & wait ${i}; done; 

	echo 'Getting certificates from letsencrypt!';

	# deletes the dummy certificates
	rm -Rf /etc/letsencrypt/live/$domains 
	rm -Rf /etc/letsencrypt/archive/$domains 
	rm -Rf /etc/letsencrypt/renewal/$domains.conf

	# Enable staging mode if needed
	#if [ $staging != "0" ]; then staging_arg="--staging"; fi
	#staging_arg="--staging";

	# Select appropriate email arg
	#case "$email" in
	#  "") email_arg="--register-unsafely-without-email" ;;
	#  *) email_arg="--email $email" ;;
	#esac

	email_arg="--register-unsafely-without-email"
	#domains_arg=-d $domains
	

	#Join $domains to -d args
	#domain_args=""
	#for domain in "${domains[@]}"; do
	#  domain_args="$domain_args -d $domain"
	#done

	certbot certonly --webroot -w /var/www/certbot $staging_arg $email_arg \
		-d $domains --rsa-key-size $rsa_key_size --agree-tos --force-renewal \
		$staging_arg

	echo 'completed2' > $path/certbot-phase2.pid
fi

echo 'Initiating renewing loop!'
while :; do certbot renew; sleep 12h & wait ${!}; done;
