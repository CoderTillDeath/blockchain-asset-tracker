/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

'use strict';
/**
 * Track the trade of a commodity from one trader to another
 * @param {org.example.biznet.Trade} trade - the trade to be processed
 * @transaction
 */
function tradeAsset(trade) {
    trade.property.owner = trade.newOwner;
    return getAssetRegistry('org.example.biznet.Property')
        .then(function (assetRegistry) {
            return assetRegistry.update(trade.property);
        });
}
/**
 * Track the trade of a commodity from one trader to another
 * @param {org.example.biznet.Transfer} transfer - the trade to be processed
 * @transaction
 */
async function transferPackage(transfer) {
	// Update handler in package
	let packRegistry = await getAssetRegistry('org.example.biznet.Package');
	let pack = transfer.package;
	pack.handler = transfer.newHandler;
	await packRegistry.update(pack);
	
	// Update all properties to new owner
	let propRegistry = await getAssetRegistry('org.example.biznet.Property');
    var i;
    for(let i = 0; i < pack.contents.length; i++) {
	    let prop = pack.contents[i];
	    prop.owner = transfer.newHandler;
	    propRegistry.update(prop);
    }
}
