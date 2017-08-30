var arr = [1, 2, 3];
arr.length; // 3
arr.length = 6;  //直接给Array的length赋一个新的值会导致Array大小的变化：
arr; // arr变为[1, 2, 3, undefined, undefined, undefined]
arr.length = 2;
arr; // arr变为[1, 2]
arr[5] = 'x';  //索引赋值时，索引超过了范围，同样会引起Array大小的变化
arr; // arr变为[1, 2, undefined, undefined, 'x']

arr.indexOf(1); // 元素1的索引为0

var arr1 = ['A', 'B', 'C', 'D', 'E', 'F', 'G'];
arr1.slice(0, 3); // 从索引0开始，到索引3结束，但不包括索引3: ['A', 'B', 'C']
arr1.slice(3); // 从索引3开始到结束: ['D', 'E', 'F', 'G']

var arr = [1, 2];
arr.push('A', 'B'); // 返回Array新的长度: 4
arr; // [1, 2, 'A', 'B']
arr.pop(); // pop()返回'B'

var arr = [1, 2];
arr.unshift('A', 'B'); // 返回Array新的长度: 4
arr; // ['A', 'B', 1, 2]
arr.shift(); // 'A'

var a = ['A', 'B', 'C'];
for (var i in a) {
    alert(i); // '0', '1', '2'
    alert(a[i]); // 'A', 'B', 'C'
}


var xiaoming = {
    name: '小明'
};
xiaoming.age; // undefined
xiaoming.age = 18; // 新增一个age属性
delete xiaoming.age; // 删除age属性
'name' in xiaoming; // true  in判断一个属性存在,可能是继承得到
xiaoming.hasOwnProperty('toString'); // false 是否是自身拥有的，而不是继承得到

for (var key in xiaoming) {
    if (xiaoming.hasOwnProperty(key)) {
        alert(key); 
    }
}

function foo(x) {
    alert(x); // 10
    for (var i=0; i<arguments.length; i++) {
        alert(arguments[i]); // 10, 20, 30
    }
}
foo(10, 20, 30);

Math.max.apply(null, [3, 5, 4]); // this绑定为null 5  apply()把参数打包成Array再传入
Math.max.call(null, 3, 5, 4); // 5   call()把参数按顺序传入

//map()方法定义在JavaScript的Array中，我们调用Array的map()方法，传入我们自己的函数，就得到了一个新的Array作为结果：
function pow(x) {
    return x * x;
}

var arr = [1, 2, 3, 4, 5, 6, 7, 8, 9];
arr.map(pow); // [1, 4, 9, 16, 25, 36, 49, 64, 81]

var arr = [1, 3, 5, 7, 9];
arr.reduce(function (x, y) {
    return x * 10 + y;
}); // 13579

var arr = ['A', '', 'B', null, undefined, 'C', '  '];
var r = arr.filter(function (s) {
    return s && s.trim(); // 注意：IE9以下的版本没有trim()方法
});
r; // ['A', 'B', 'C']

var arr = [10, 20, 1, 2];
arr.sort(function (x, y) {
    if (x < y) {
        return 1;
    }
    if (x > y) {
        return -1;
    }
    return 0;
}); // [20, 10, 2, 1]

//类
function Student(name) {
    this.name = name;
    this.hello = function () {
        alert('Hello, ' + this.name + '!');
    }
}
var xiaoming = new Student('小明');
xiaoming.name; // '小明'
xiaoming.hello(); // Hello, 小明!


//改进
function Student(props) {
    this.name = props.name || '匿名'; // 默认值为'匿名'
    this.grade = props.grade || 1; // 默认值为1
}

Student.prototype.hello = function () {     //对象的hello函数实际上只需要共享同一个函数就可以了，这样可以节省很多内存。
    alert('Hello, ' + this.name + '!');
}

function createStudent(props) {
    return new Student(props || {})
}

var xiaoming = createStudent({
    name: '小明'
});

xiaoming.grade; // 1


//继承
function inherits(Child, Parent) {
    var F = function () {};
    F.prototype = Parent.prototype;
    Child.prototype = new F();
    Child.prototype.constructor = Child;
}

function PrimaryStudent(props) {
    Student.call(this, props);
    this.grade = props.grade || 1;
}

// 实现原型继承链:
inherits(PrimaryStudent, Student);
// 绑定其他方法到PrimaryStudent原型:
PrimaryStudent.prototype.getGrade = function () {
    return this.grade;
};


// class 类
class Student1 {
    constructor(name) {
        this.name = name;
    }

    hello() {
        alert('Hello, ' + this.name + '!');
    }
}

class PrimaryStudent extends Student1 {
    constructor(name, grade) {
        super(name); // 记得用super调用父类的构造方法!
        this.grade = grade;
    }

    myGrade() {
        alert('I am at grade ' + this.grade);
    }
}