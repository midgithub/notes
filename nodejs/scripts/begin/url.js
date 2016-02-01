> url
{ parse: [Function: urlParse],
  resolve: [Function: urlResolve],
  resolveObject: [Function: urlResolveObject],
  format: [Function: urlFormat],
  Url: [Function: Url] }

> url.parse('http://www.imooc.com/course/list')
Url {
  protocol: 'http:',
  slashes: true,
  auth: null,
  host: 'www.imooc.com',
  port: null,
  hostname: 'www.imooc.com',
  hash: null,
  search: null,
  query: null,
  pathname: '/course/list',
  path: '/course/list',
  href: 'http://www.imooc.com/course/list' }

> url.parse('http://www.imooc.com:8080/course/list?from=scott&&course=node#floor1')
Url {
  protocol: 'http:',
  slashes: true,
  auth: null,
  host: 'www.imooc.com:8080',
  port: '8080',
  hostname: 'www.imooc.com',
  hash: '#floor1',
  search: '?from=scott&&course=node',
  query: 'from=scott&&course=node',
  pathname: '/course/list',
  path: '/course/list?from=scott&&course=node',
  href: 'http://www.imooc.com:8080/course/list?from=scott&&course=node#floor1' }

> url.parse('http://www.imooc.com:8080/course/list?from=scott&&course=node#floor1',true) //后面两个参数默认false
Url {
  protocol: 'http:',
  slashes: true,
  auth: null,
  host: 'www.imooc.com:8080',
  port: '8080',
  hostname: 'www.imooc.com',
  hash: '#floor1',
  search: '?from=scott&&course=node',
  query: { from: 'scott', '': '', course: 'node' },
  pathname: '/course/list',
  path: '/course/list?from=scott&&course=node',
  href: 'http://www.imooc.com:8080/course/list?from=scott&&course=node#floor1' }

> url.parse('//imooc.com/course/list',true)
Url {
  protocol: null,
  slashes: null,
  auth: null,
  host: null,
  port: null,
  hostname: null,
  hash: null,
  search: '',
  query: {},
  pathname: '//imooc.com/course/list',
  path: '//imooc.com/course/list',
  href: '//imooc.com/course/list' }

> url.parse('//imooc.com/course/list',true,true)
Url {
  protocol: null,
  slashes: true,
  auth: null,
  host: 'imooc.com',
  port: null,
  hostname: 'imooc.com',
  hash: null,
  search: '',
  query: {},
  pathname: '/course/list',
  path: '/course/list',
  href: '//imooc.com/course/list' }

> url.format({
  protocol: 'http:',
  slashes: true,
  auth: null,
  host: 'www.imooc.com:8080',
  port: '8080',
  hostname: 'www.imooc.com',
  hash: '#floor1',
  search: '?from=scott&&course=node',
  query: 'from=scott&&course=node',
  pathname: '/course/list',
  path: '/course/list?from=scott&&course=node',
  href: 'http://www.imooc.com:8080/course/list?from=scott&&course=node#floor1' })
'http://www.imooc.com:8080/course/list?from=scott&&course=node#floor1'

url.resolve('http://imooc.com','/course/list')
'http://imooc.com/course/list'