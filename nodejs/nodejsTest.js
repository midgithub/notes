//this is nodejs test
"use strict"; 

// var modul = require("./hello2");
// var name = "qing";
// modul.get(name);

function main () {
	console.log("main");
	//globalVar();
	serverTest();
	asyncTest();
}
function globalVar () {
	//js在浏览器中有且仅有一个全局window对象。而在Node.js环境中,也有唯一的全局对象global
	//__filename 表示当前正在执行的脚本的文件名。它将输出文件所在位置的绝对路径
	//__dirname 表示当前执行脚本所在的目录
	// setTimeout(cb, ms) 全指定的毫秒(ms)数后执行指定函数(cb)。只执行一次指定函数。返回一个代表定时器的句柄值。
	// setInterval(cb, ms) 同上，会不停地调用函数，直到 clearInterval() 被调用或窗口被关闭。
	//clearTimeout( t ) 用于停止一个之前创建的定时器
	//console.time("获取数据"); ... console.timeEnd('获取数据'); 获取数据:1ms
	//process

	// for ( var k in global) {
	// 	console.log(k+" "+global[k]);
	// }
	console.log(global.console); 

	//Node.js进程
	console.log("Node.js进程"); 
	console.log(global.process); //true
}

//简单web服务
function serverTest () {
	var http = require("http");
	var server = http.createServer(function  (req,res) {
		// 获得HTTP请求的method和url:
    	console.log(request.method + ': ' + request.url);
		res.writeHead(200,{"Content-Type":"text/plain"});
		res.end("hello node");
	});
	server.listen(1337,"127.0.0.1");
	console.log("server running at http://127.0.0.1");
}

//异步 非阻塞编程
function asyncTest () {
	//Node.js 异步编程依托于回调来实现,这样在执行代码时就没有阻塞或等待文件 I/O 操作。
	//这就大大提高了 Node.js 的性能，可以处理大量的并发请求。

	var fs = require("fs");

	//阻塞代码 同步读取
	// var data = fs.readFileSync('test.txt');
	// console.log(data.toString());
	// console.log("程序执行结束!");

	//非阻塞代码 异步读取
	fs.readFile('test.txt', function (err, data) {
	    if (err) return console.error(err);
	    console.log(data.toString());
	});
	console.log("程序执行结束!");
}

//事件循环
function eventTest () {
	//Node.js 是单进程单线程应用程序，但是通过事件和回调支持并发，所以性能非常高。
	//Node.js 的每一个 API 都是异步的，并作为一个独立线程运行，使用异步函数调用，并处理并发。
	//Node.js 基本上所有的事件机制都是用设计模式中观察者模式实现。
	//Node.js 单线程类似进入一个while(true)的事件循环，直到没有事件观察者退出，每个异步事件都生成一个事件观察者，如果有事件发生就调用该回调函数.

	//Node.js 使用事件驱动模型，当web server接收到请求，就把它关闭然后进行处理，然后去服务下一个web请求。
	//当这个请求完成，它被放回处理队列，当到达队列开头，这个结果被返回给用户。
	//这个模型非常高效可扩展性非常强，因为webserver一直接受请求而不等待任何读写操作。（这也被称之为非阻塞式IO或者事件驱动IO）
	//在事件驱动模型中，会生成一个主循环来监听事件，当检测到事件时触发回调函数。


	// on(event, listener)
	// 为指定事件注册一个监听器，接受一个字符串 event 和一个回调函数。

	// once(event, listener)
	// 为指定事件注册一个单次监听器，即 监听器最多只会触发一次，触发后立刻解除该监听器。

	// addListener(event, listener)
	// 为指定事件添加一个监听器到监听器数组的尾部。

	// removeListener(event, listener)
	// 移除指定事件的某个监听器，监听器 必须是该事件已经注册过的监听器

	// removeAllListeners([event])
	// 移除所有事件的所有监听器， 如果指定事件，则移除指定事件的所有监听器。

	// setMaxListeners(n)
	// 默认情况下， EventEmitters 如果你添加的监听器超过 10 个就会输出警告信息。 setMaxListeners 函数用于提高监听器的默认限制的数量

	// listeners(event)
	// 返回指定事件的监听器数组

	// emit(event, [arg1], [arg2], [...])
	// 按参数的顺序执行每个监听器，如果事件有注册监听返回 true，否则返回 false

	var events = require('events');
	// 创建 eventEmitter 对象
	var eventEmitter = new events.EventEmitter();

	// 绑定 connection 事件处理程序 
	eventEmitter.on('connection', listner1 );
	// 创建事件处理程序 (监听器)
	var listner1  = function connected() {
	   console.log('连接成功。');
	  
	   // 触发 data_received 事件 
	   eventEmitter.emit('data_received' ,'qingsen');
	}
	 
	// 使用匿名函数绑定 data_received 事件
	eventEmitter.on('data_received', function(name){
	   console.log('数据接收成功。' + name);
	});

	// 触发 connection 事件 eventEmitter.emit('eventName');
	//延迟1000 毫秒以后向 eventEmitter 对象发送事件 connection，此时会调用 connection 的监听器
	var t = setTimeout(function() {   //返回一个代表定时器的句柄值
		eventEmitter.emit('connection');
	}, 1000); 

	console.log("程序执行完毕。");
	// EventEmitter 定义了一个特殊的事件 error，它包含了错误的语义，我们在遇到 异常的时候通常会触发 error 事件。
	// 当 error 被触发时，EventEmitter 规定如果没有响 应的监听器，Node.js 会把它当作异常，退出程序并输出错误信息。
	// 我们一般要为会触发 error 事件的对象设置监听器，避免遇到错误后整个程序崩溃。
}

main()
//module.exports是个空对象
module.exports = {
	main: main,
	init: init   //字典最后一个最好不要写 逗号
}; //模块对外输出变量,可以是任意对象、函数、数组等等