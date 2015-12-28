"use strict";
function alert (argument) {
	console.log(argument);
}
alert(false == 0); //true  它会自动转换数据类型再比较，很多时候，会得到非常诡异的结果；
alert(false === 0); //false 它不会自动转换数据类型，如果数据类型不一致，返回false，如果一致，再比较。

//NaN 这个特殊的Number与所有其他值都不相等，包括它自己 唯一能判断NaN的方法是通过isNaN()函数
alert(isNaN(NaN));

//undefined (原生类型未申明)
//null (对象为空)
//0 开始索引 typeof 123;'number'

//用parseInt()或parseFloat()来转换任意类型到number
//用String()来转换任意类型到string

function arrTest () {
	var arr1 = new Array(3); //当只有一个数时表示创建数组的大小,未初始化，值都是  undefined
	for (var i = 0; i < arr1.length; i++) {
	   arr1[i] = i+1;
	}
	var arr2 = new Array(4,5,6);  

	var concat = arr1.concat(arr2,7,[8,9]);  //连接两个或更多的数组，并返回连接后的新数组，原数组没变化
	alert(concat);  //[1,2,3,4,5,6,7,8,9] 调用toString(),默认 ，为分割符

	var join = concat.join("+");  //将数组元素以+为分割符连接元素，返回拼接的 字符串
	alert(join);  // 1+2+3+4+5+6+7+8+9 

	var pop = concat.pop();  //删除并返回最后一个元素
	alert(pop);

	var push = concat.push(10);  //向数组的末尾添加一个或更多元素，并返回新的长度。
	alert(push);

	var shift = concat.shift();  //删除并返回数组的第一个元素
	alert(shift);

	//向数组的头添加一个或更多元素，并返回新的长度。
	var unshift = concat.unshift(0,[1,2]);  // [1,2] 做为一个整体，reverse 的时候里面1,2不会反过来
	alert(concat);  //10,9,8,7,6,5,4,3,2,1,2,0

	concat.reverse();
	alert(concat);
}

function strTest () {
	var str = "123456";
	for (var i = 0; i < str.length; i++) {
		alert("str["+i+"]:"+str[i]);  //字符串是不可变的，如果对字符串的某个索引赋值，不会有任何错误但是也没有任何效果：str[1] =xx
	};
	alert(str.indexOf('3')); //2
	alert(str.substring(1,3)); //23 和py一样也是左闭右开
}

function dicTest () {
	var xiaoming = {
	    name: '小明',
	    birth: 1990
	};
	xiaoming.num = 37; //动态添加
	for ( var k in xiaoming) {
		console.log(k+" "+xiaoming[k]);
	}
	delete xiaoming.num; //删除属性
	alert('name' in xiaoming); // true

	//Map  无论这个表有多大，查找速度都不会变慢
    var m = new Map([['Michael', 95], ['Bob', 75], ['Tracy', 85]]); //二维数组
    m.get('Michael'); //95
    m.set("sen",37); // 添加新的key-value
    m.has('Michael'); // 是否存在key 'Michael': true
    //m.delete('Michael');
    m.set("sen",73);//一个key只能对应一个value，所以，多次对一个key放入value，后面的值会把前面的值冲掉：

    //Set  一组key的集合，但不存储value。由于key不能重复，所以在Set中没有重复的key
    var s = new Set([1, 2, 3, 3, '3']); //{1, 2, 3, "3"}
    s.add(4);
    s.delete(4);

    //遍历Map和Set 无法使用下标遍历循环

    var a = ['A', 'B', 'C'];
	var s = new Set(['A', 'B', 'C']);
	var m = new Map([[1, 'x'], [2, 'y'], [3, 'z']]);
	for (var value of a) { // 遍历Array
	    alert(value);
	}
	for (var value of s) { // 遍历Set
	    alert(value);
	}
	for (var value of m) { // 遍历Map
	    alert(value[0] + '=' + value[1]);
	}
	//for (var value of a)  for ( var key in xiaoming) of和in的区别就是一个是value一个是key
}

function funTest () {
	//Map 
	var pow = function (x) {
	    return x * x;
	}

	var arr = [1, 2, 3, 4, 5, 6, 7, 8, 9];
	alert(arr.map(pow)); // [1, 4, 9, 16, 25, 36, 49, 64, 81]

	//Reduce 
	arr.reduce(function (x, y) {
	    return x + y;
	}); // 25

	//
	var r = arr.filter(function (x) {
	    return x % 2 !== 0;
	}); // [1,3,5,7,9]

	//
	arr.sort(function (x, y) {
	    if (x < y) {
	        return 1;
	    }
	    if (x > y) {
	        return -1;
	    }
	    return 0;
	}); // [9,8..

	//闭包
	function count() {
	    var arr = [];
	    for (var i=1; i<=3; i++) {
	        arr.push(function () {
	            alert(i * i);
	        });
	    }
	    return arr;
	}

	var fs = count();
	fs[0]();fs[1]();fs[2](); //返回的函数引用了变量i，但它并非立刻执行。等到3个函数都返回时，它们所引用的变量i已经变成了4，因此最终结果为16

	//创建一个匿名函数并立刻执行
	(function (x) {
	    alert(x * x);
	})(3);

	//生成器 
	// function* fib(max) {
	//     var
	//         t,
	//         a = 0,
	//         b = 1,
	//         n = 1;
	//     while (n < max) {
	//         yield a; //每次调用next()的时候执行，遇到yield语句返回，再次执行时从上次返回的yield语句处继续执行。
	//         t = a + b;
	//         a = b;
	//         b = t;
	//         n ++;
	//     }
	//     return a;
	// }
	// var f = fib(5);
	// f.next(); // {value: 0, done: false}
	// f.next(); // {value: 1, done: false}
	// f.next(); // {value: 1, done: false}
	// f.next(); // {value: 2, done: false}
	// f.next(); // {value: 3, done: true}

	// for (var x of fib(5)) {
	//     alert(x); // 依次输出0, 1, 1, 2, 3
	// }
}

function regExpTest (argument) {
	//正则表达式：两种写法
    var re1 = /^(\d{3})-(\d{3,8})$/;
    var re2 = new RegExp('ABC\\-001'); //注意字符串的转义问题
    re1.test('010-12345'); // true
    re1.exec('010-12345'); //分组 ['010-12345', '010', '12345'] 第一个元素始终是原始字符串本身，后面的字符串表示匹配成功的子串。
   
    //全局匹配：
    var s = 'JavaScript, VBScript, JScript and ECMAScript';
    var re=/[a-zA-Z]+Script/g;

    // 使用全局匹配:
    //每次运行exec(),正则表达式本身会更新lastIndex属性,表示上次匹配到的最后索引
    re.exec(s); // ['JavaScript']
    re.lastIndex; // 10

    re.exec(s); // ['VBScript']
    re.lastIndex; // 20
}

function jsonTest (argument) {
	//序列化
	var xiaoming = {
	    name: '小明',
	    age: 14,
	    gender: true,
	    grade: null,
	    'middle-school': '\"W3C\" Middle School',
	    skills: ['JavaScript', 'Java', 'Python', 'Lisp']
	};
	alert(JSON.stringify(xiaoming));
	alert(JSON.stringify(xiaoming, null, '  ')) ;
	alert(JSON.stringify(xiaoming, ['name', 'skills'], '  ')); //筛选对象的键值

	var convert = function (key, value) {
	    if (typeof value === 'string') {
	        return value.toUpperCase();
	    }
	    return value;
	}
	alert(JSON.stringify(xiaoming, convert, '  ')) ; //对 键值 处理

	//反序列化 JSON.parse()把str变成一个JavaScript对象 ,接收函数，用来转换解析出的属性
	alert(JSON.parse('{"name":"小明","age":14}', function (key, value) {
	    // 把number * 2:
	    if (key === 'name') {
	        return value + '同学';
	    }
	    return value;
	})); // Object {name: '小明同学', age: 14}
}

function classTest () {
	// 实例原型用__proto__  , 类用prototype
	// 方法一（无继承）
	var Student1 = {
	    name: 'Robot',
	    height: 1.2,
	    run: function () {
	        alert(this.name + ' is running...');
	    }
	};

	function newStudent(name) {
	    var s = Object.create(Student1);
	    s.name = name;
	    return s;
	}

	var xiaoming = newStudent('小明');
	xiaoming.run(); // 小明 is running...
	alert(xiaoming.__proto__ === Student1); // tru
	alert(xiaoming instanceof Object); //tru 但是 xiaoming instanceof Student1 报错
	//xiaoming.__proto__  = {...} 不要直接用obj.__proto__ 去改变一个 对象的原型


	//方法二 (构造函数)
	//当我们用obj.xxx访问一个对象的属性时，JavaScript引擎先在当前对象上查找该属性，如果没有找到，就到其原型对象上找
	//如果还没有找到，就一直上溯到 Object.prototype 对象，最后如果还没有找到，就只能返回undefined
	//xiaoming -→ Student.prototype --> Object.prototype --> null
	function Student2(name) {
	    this.name = name || '匿名';
	    // this.hello = function () {
	    //     alert('Hello, ' + this.name + '!');
	    // }
	    //return this 默认返回不用写
	}
	Student2.prototype.hello = function () {
	    alert('Hello, ' + this.name + '!');
	};
	var s1 = new Student2('小明');
	s1.name; // '小明'
	s1.hello(); // Hello, 小明!
	alert(s1 instanceof Student2); //true xiaoming ---> Student2.prototype ---> Object.prototype ---> null
	alert(s1.constructor === Student2.prototype.constructor); //true
	alert(Student2.prototype.constructor === Student2); // true)
	

	//继承
	function PrimaryStudent(name) {
	    // 调用Student2构造函数，绑定this变量:
	    this.grade = 1;
	    Student2.call(this, name);
	}
	var s2 = new PrimaryStudent()
	alert(s2 instanceof Student2); //false 此时 s2 ----> PrimaryStudent.prototype ----> Object.prototype ----> null
	// PrimaryStudent.prototype = Student.prototype;
	// 如果这样的话，PrimaryStudent和Student共享一个原型对象，那还要定义PrimaryStudent干啥？

	function F() {
	}
	F.prototype = Student2.prototype; // 把F的原型指向Student.prototype:
	var s3 = new PrimaryStudent()
	alert(s3 instanceof Student2);  //false

	//实现继承
	PrimaryStudent.prototype = F.prototype; //new F()
	var s4 = new PrimaryStudent()
	alert(s4 instanceof Student2); //true 此时 s4 --> PrimaryStudent.prototype -->  Student2.prototype -->Object.prototype --> null

	//上面改写 隐藏F 
	// function jic(Child, Parent) {
	//     var F = function () {};
	//     F.prototype = Parent.prototype;
	//     Child.prototype = F.prototype;
	// }
	// jic(PrimaryStudent, Student2);
}

function main () {
	//arrTest()
	//strTest()
	//dicTest()
	//funTest()
	//regExpTest()
	//jsonTest()
	classTest()
}
main()

