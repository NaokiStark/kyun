//Logger
var log4js = require('log4js');
log4js.configure({
  appenders: {
    out: { type: 'stdout' },
    app: { type: 'file', filename: 'last.log' }
  },
  categories: {
    default: { appenders: [ 'out', 'app' ], level: 'debug' }
  }
});
var l = log4js.getLogger("");
l.level = 'debug';



//Array object support
Array.prototype.indexOf = function (searchElement /*, fromIndex */ ) {
    "use strict";
    if (this == null) {
        throw new TypeError();
    }
    var t = Object(this);
    var len = t.length >>> 0;
    if (len === 0) {
        return -1;
    }
    var n = 0;
    if (arguments.length > 1) {
        n = Number(arguments[1]);
        if (n != n) { // para verificar si es NaN
            n = 0;
        } else if (n != 0 && n != Infinity && n != -Infinity) {
            n = (n > 0 || -1) * Math.floor(Math.abs(n));
        }
    }
    if (n >= len) {
        return -1;
    }
    var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
    for (; k < len; k++) {
        if (k in t && t[k] === searchElement) {
            return k;
        }
    }
    return -1;
}
//Server constants
const wsPort = 8085;

const dbHost = 'localhost';

const dbPort = 3306;

const dbUser = 'root'; //Fill w user

const dbPassword = ''; //

const dbDatabase = 'onics_fire';

//DB const

const mysql = require('mysql');


//Socket

l.info('Starting WebSocket on port: ' + wsPort);
const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: wsPort/*, server: 'httpsServer'*/ });


function noop() {}

const interval = setInterval(function ping() {
  wss.clients.forEach(function each(ws) {
    if (ws.isAlive === false){
    	l.info(ws.cid+": Client disconnected");
    	var index = clients.indexOf(ws);

    	if (index > -1) {
    		clients.splice(index, 1);
		}

    	return ws.terminate();
    }

    ws.isAlive = false;
    ws.ping(noop);
  });
}, 30000);

//other

var intervalDaemon = 0;

//{ws, hash}

var clients = [];

//Mysql pool

var pool;

var actualId = 0;
//

wss.on('connection', Connection);


function Connection(ws, req){


	ws.cid = actualId;

	actualId++;

	l.info("New connection from: " + req.connection.remoteAddress + " | cid: "+ws.cid);

	ws.on('error', function(e){
		l.info(this.cid+": "+ e);		
	});
	ws.on('pong', heartbeat);
	//Wait for auth	
	ws.on('message', onMessage);	


}

function heartbeat() {
  this.isAlive = true;
}

function onMessage(message){

	if(clients.indexOf(this) < 0){
		l.info(this.cid +": Force auth");
		clients.push(this);
		auth(this, message);
		return;
	}

	l.info(this.cid + ": "+message);
	processMessage(this, message);
}

function processMessage(ws, message){

	//broadcast test
	 wss.clients.forEach(function each(client) {
      if (client !== ws && client.readyState === WebSocket.OPEN) {
        client.send(ws.cid+ ": "+ message);
      }
    });
}

function send(ws, message){
	
	ws.send(JSON.stringify(message));

}

function auth(ws, message){

	try{
		var m = JSON.parse(message); //ToDo clean string

	}
	catch(err){

		l.info(ws.cid+": Malformed packet, terminating");

		ws.send(JSON.stringify({status: 0, data: {cid: ws.cid, text: 'Invalid Auth.'}}));
		
		var index = clients.indexOf(ws);
		
		if (index > -1) {
    		clients.splice(index, 1);
		}

		ws.terminate();

		return;
	}

	var index = clients.indexOf(ws);

	if (index < 0) {
		return;
	}

	if(!typeof(clients[index]) == 'undefined'){
		return;
	}
	/*
	dbQuery("SELECT wshash FROM users WHERE wshash = '"+m.hash+"' AND id = '"+m.id+"'", function(r){

		if(r.length == 0){
			ws.send(JSON.stringify({status: 0, data: {text: 'Invalid Auth.'}}));
			clients.splice(index);
			ws.terminate();
			return;
		}
		//console.log(index)

		clients[index].hash = r[0].wshash;

		ws.send(JSON.stringify({status: 1, data: {text: 'ok'}}));

	})*/

	l.info(ws.cid+": Bypass auth. ToDo");
	//Add extra data


	ws.send(JSON.stringify({status: 1, data: {cid: ws.cid, text: 'ok'}}));
}

function dbQuery(query, callback){

	pool.getConnection(function(err, connection){

		if(err){
			console.log(err);
			return;
		}

		connection.query(query, [], function(err, result){
			connection.release();
			if(err){
				console.log(err);
				return;
			}

			callback(result);
		});
	});
}

function update(){


	
	var query = "SELECT notifications.*, users.avatar as avatar, users.user, u.wshash as hashClient \
	FROM notifications \
	LEFT JOIN users ON notifications.interventor = users.id \
	LEFT JOIN users as u ON notifications.userid = u.id\
	WHERE wsqueue = 0 ORDER BY id DESC LIMIT 5";

	dbQuery(query, sendNotifications);

}
/*
function sendNotifications(r){
	//console.log(r);
	let sended = 0;
	for (var i = r.length - 1; i >= 0; i--) {
		if(r[i]['hashClient'].length < 1){
			//ToDo: mark as sended, or remove
			let query = "UPDATE notifications SET wsqueue = '1' WHERE id = '"+r[i].id+"'";
			dbQuery(query, function(){});
			continue; 
		}

		for (var c = clients.length - 1; c >= 0; c--) {
			if(clients[c].hash == r[i]['hashClient']){
				r[i].message = createMessage(r[i].type, r[i].user);
				try{
					clients[c].send(JSON.stringify(r[i]));						
				}
				catch(e){
					clients.splice(c, 1);
					break;
				}
				sended++;
			}

		}
		let query = "UPDATE notifications SET wsqueue = '1' WHERE id = '"+r[i].id+"'";
		dbQuery(query, function(){});
		//ToDo: mark as sended, or remove
	}
	//console.log('sended ' + sended + ' notifications');
	
}*/



function makeMySqlConnectionAndDaemon(){
	l.info('DB not implemented yet.');
	/*
	pool = mysql.createPool({
		host: dbHost,
		port: dbPort,
		user: dbUser,
		password: dbPassword,
		database: dbDatabase
	});

	ToDo: mysql database
	*/



	//intervalDaemon = setInterval(update, 3000); Not too
}


//Start pool
makeMySqlConnectionAndDaemon();
