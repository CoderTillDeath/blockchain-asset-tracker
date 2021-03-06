#!/bin/bash

echo "Be sure to run the 'startFabric.sh' script first!"
composer network install --card PeerAdmin@hlfv1 --archiveFile oracle-asset-track@0.0.8.bna
#composer network upgrade -c PeerAdmin@hlfv1 -n oracle-asset-track -V 0.0.8
composer network start --networkName oracle-asset-track --networkVersion 0.0.8 --networkAdmin admin --networkAdminEnrollSecret adminpw --card PeerAdmin@hlfv1 --file networkadmin.card
