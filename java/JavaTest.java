//源文件名必须和类名相同,一个源文件中只能有一个public类
//javac 命令用于将 java 源文件编译为 class 字节码文件 javac -encoding UTF-8 JavaTest.java
//java 命令可以运行 class 字节码文件
//单继承,单继承,同c#
//语音本身不支持多平台，因为有虚拟机

package com.sen.androidfirst //包声明，及命名空间，在源文件的首行
import java.io.*; //在package 后面，使用某命名空间，using 命令编译器载入java_installation/java/io路径下的所有类
public class JavaTest {

    public static void main(String []args) {
        System.out.println("Hello World");
    }
}