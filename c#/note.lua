--随机金币
function RunWidget:golding(dt)
    self.dtGold = self.dtGold + dt
    if self.dtGold < self.goldTime/self.speed then
        return
    end
    self.dtGold = 0

    local rand = math.random(1, 5)
    rand = 1

    local file = string.format("gold_%d.csb", rand)
    local node = cc.CSLoader:createNode(file)
    local nodes = Helper.colWidget(node)

    local action = cc.CSLoader:createTimeline(file) -- cocostudio 动画
    action:play("moving", false)
    node:runAction(action)

    local prefix, body
    local res, tag
    for name, n in pairs(nodes) do
        res = true
        if 1 == string.find(name, 'g') then
            tag = TAG_GOLD
        elseif 1 == string.find(name, 's') then
            tag = TAG_SILVER
        else
            res = false
        end

        if res then
            body = cc.PhysicsBody:createCircle(34, cc.PhysicsMaterial(0.00, 0, 0))
            body:setTag(tag)
            body:setGravityEnable(false)  --设置body是否受引力影响
            body:setContactTestBitmask(TEST_MONEY) --通知掩码
            body:setCategoryBitmask(CONTACT_MONEY)  ----分类掩码
            body:setCollisionBitmask(MASK_MONEY)  ----碰撞掩码

            n:setPhysicsBody(body)
        end
    end

    self.root:addChild(node)
end

--设置重力加速度
    local world = self:getScene():getPhysicsWorld()
    world:setGravity(cc.p(0, self.gravity*self.speed))

    --设置碰撞监听
    local listener = cc.EventListenerPhysicsContact:create()
    listener:registerScriptHandler(
        function(contact)
            return self:onContactBegin(contact)
        end, 
        cc.Handler.EVENT_PHYSICS_CONTACT_BEGIN
    );

    local eventDispatcher = self.widget:getEventDispatcher()
    eventDispatcher:addEventListenerWithSceneGraphPriority(listener, self.widget);
    
    --碰撞检测
--注：回调中更改状态导致扰动问题
function RunWidget:onContactBegin(contact)
    local shapeA = contact:getShapeA()
    local shapeB = contact:getShapeB()

    local bodyA = shapeA:getBody()
    local bodyB = shapeB:getBody()

    local tagA = bodyA:getTag()
    local tagB = bodyB:getTag()

    if tagA==TAG_ROLE and tagB==TAG_EARTH then
        self:onContactRoleEarth()
    elseif tagA==TAG_EARTH and tagB==TAG_ROLE then
        self:onContactRoleEarth()
    elseif tagA==TAG_ROLE and tagB==TAG_BALK then
        self:onContactRoleBalk(shapeB)
    elseif tagA==TAG_BALK and tagB==TAG_ROLE then
        self:onContactRoleBalk(shapeA)
    elseif tagA==TAG_ROLE and (tagB==TAG_GOLD or tagB==TAG_SILVER) then
        self:onContactRoleMoney(tagB, bodyB:getNode())
    elseif tagB==TAG_ROLE and (tagA==TAG_GOLD or tagA==TAG_SILVER) then
        self:onContactRoleMoney(tagA, bodyA:getNode())
    end

    return true
end

package.loaded["src/Event"] = nil  
require('UITable')  --require:多次调用返回同一个指针  package: 返回不同指针


local sig = "(I)I"
local luaj = require "cocos.cocos2d.luaj"
luaj.callStaticMethod("com/bluehat/rbugs/JniHelper", "getVersionCode", {1}, sig)

--java 与 c++ 通过JNI 互调
--

local webSize = cc.size()
local webView = ccexp.WebView:create()
webView:setContentSize(webSize)
webView:setPosition(cc.p(viewSize.width/2,sizeDown.height+webSize.height/2))
webView:loadURL(Const.NOTICE_URL)
webView:setScalesPageToFit(true)


local shader = cc.GLProgramCache:getInstance():getGLProgram(cc.SHADER_UI_GRAY_SCALE)
node.bg:setGLProgram(shader)


local container = self.widget.scrollview_label:getInnerContainer()
local sizeContainer = container:getContentSize()
local sizeView = self.widget.scrollview_label:getContentSize()

local height = sizeContainer.height - sizeView.height

self.widget.scrollview_label:addEventListener(function (sender, eventType)
    if eventType == ccui.ScrollviewEventType.scrolling then
        local y = container:getPositionY() 
        local percent = 100+y/height*100
        self.widget.slider:setPercent(percent)
    end
end)
self.widget.slider:addEventListener(function (sender,eventType)
    if eventType == ccui.SliderEventType.percentChanged then
        local percent =100-sender:getPercent()
        local y = height*percent/100
        container:setPositionY(-y)
    end
end)


node.btn_buy:setSwallowTouches(false)
local move = false
node.btn_buy:addTouchEventListener(function(sender, eventType)
    if eventType == ccui.TouchEventType.ended then
        if not move then 
            AudioManager.playBtnEffect()
            if Const.SITEM.GOODS == self.nType then
                if Item.getNum(itemID) >= 1 then
                else
                    self:showItem(itemID)
                end
            else
                self:showItem(itemID)
            end
        end 
    elseif eventType == ccui.TouchEventType.began then
        move = false
    elseif eventType == ccui.TouchEventType.moved then
        move = true
    end
end)

每帧调用的，
void scheduleUpdateWithPriority (int priority)
void scheduleUpdateWithPriorityLua (int nHandler,int priority)
void unscheduleUpdate()

指定调用间隔时间的，
unsigned int scheduleScriptFunc (unsigned int nHandler, float fInterval, bool bPaused)
还有取消定时器事件
void unscheduleScriptEntry (unsigned int uScheduleScriptEntryID)

* obj：obj是max或maya默认可以导出的格式，缺点就是不支持动画的导出。
* c3t：c3t文件是通过FBX模型文件转换后生成的Json格式的文件，使用c3t格式的目的是方便用户进行模型数据的查看和版本比较，由于c3t文件是Json格式的，所以它的文件体积比较大，载入速度也比较慢，通常在实际游戏中不提倡使用。
* c3b：c3b是二进制文件，数据的内容与c3t文件是一样的，不同的是c3b文件体积小，加载速度快，提倡在实际游戏开发中使用。
local sprite = cc.Sprite3D:create("Sprite3DTest/boss1.obj")
sprite:setTexture("Sprite3DTest/boss.png")
action = cc.TintBy:create(2, 0, -255, -255)
local listener = cc.EventListenerTouchOneByOne:create()  --OneByOne ： touch 单点
listener:setSwallowTouches(true)
listener:registerScriptHandler(function (touch, event)
    local target = event:getCurrentTarget()
    local rect   = target:getBoundingBox()
    if cc.rectContainsPoint(rect, touch:getLocation()) then
        print(string.format("sprite3d began... x = %f, y = %f", touch:getLocation().x, touch:getLocation().y))
        target:setOpacity(100)
        return true
    end

    return false  --不执行后面的moved和 ended
end,cc.Handler.EVENT_TOUCH_BEGAN )

listener:registerScriptHandler(function (touch, event)
    print("sprite3d onMoved")
    local target = event:getCurrentTarget()
    local x,y = target:getPosition()
    target:setPosition(cc.p(x + touch:getDelta().x, y + touch:getDelta().y))
end, cc.Handler.EVENT_TOUCH_MOVED)

listener:registerScriptHandler(function (touch, event)
    local target = event:getCurrentTarget()
    print("sprite3d onTouchEnd")
    target:setOpacity(255)
end, cc.Handler.EVENT_TOUCH_ENDED)

local eventDispatcher = layer:getEventDispatcher()
eventDispatcher:addEventListenerWithSceneGraphPriority(listener, sprite1)
eventDispatcher:addEventListenerWithSceneGraphPriority(listener:clone(), sprite2)  --事件克隆


local listener = cc.EventListenerTouchAllAtOnce:create()  --AllAtOnce ： touches 多点
 listener:registerScriptHandler(Sprite3DBasicTest.onTouchesEnd,cc.Handler.EVENT_TOUCHES_ENDED )
function Sprite3DBasicTest.onTouchesEnd(touches, event)
    for i = 1,table.getn(touches) do
        local location = touches[i]:getLocation()
        Sprite3DBasicTest.addNewSpriteWithCoords(Helper.currentLayer, location.x, location.y )
    end
end

--
--3dMax 可以导出 .obj(主流的3D编辑器都支持它,能够很简单被解析。但是它不支持动画特性,cocos编辑器不能使用该格式文件，代码可以) 
--和 .fbx 文件(要通过fbx-conv 工具转换为 c3b或c3t,cocos编辑器能使用该格式文件)
local sprite = cc.Sprite3D:create("Sprite3DTest/orc.c3b")
local animation = cc.Animation3D:create("Sprite3DTest/orc.c3b")
local animate = cc.Animate3D:create(animation)
animate:setQuality(x) --0:stop 1:low 2:high 
local repeate = cc.RepeatForever:create(animate)
repeate:setTag(110)
sprite:runAction(repeate)

local repAction = sprite:getActionByTag(110)
local animate3D = repAction:getInnerAction()

local animate = cc.Animate3D:create(animation, 0.0, 1.933) --开始时间,间隔时间
self._state = State.SWIMMING
sprite:runAction(cc.RepeatForever:create(animate))
self._swim = animate
self._swim:retain()

self._hurt = cc.Animate3D:create(animation, 1.933, 2.8)
self._hurt:retain()

sprite:getAttachNode("Bip001 R Hand"):addChild(sp)
sprite:removeAllAttachNode()

local animCache = cc.AnimationCache:getInstance()
animCache:addAnimationsWithFile("xxx.plist")
--缓存动画
table.insert(self.animNames, name)
for i=1, #animationCache do
    animCache:removeAnimation(self.animNames[i])
end
local animation = animationCache:getAnimation(name)
animCache:addAnimation(animation, name)



--3d物理 刚体(3dobj) -> 组件 -> 3dSprite 添加组件
local physics3DWorld = scene:getPhysics3DWorld()
physics3DWorld:setDebugDrawEnable(false)
scene:setPhysics3DDebugCamera(self._camera)

local rbDes = {}
rbDes.originalTransform = cc.mat4.translate(cc.mat4.createIdentity(), self._camera:getPosition3D())
rbDes.mass = 1.0
rbDes.shape = cc.Physics3DShape:createBox(cc.vec3(0.5, 0.5, 0.5))

local sprite = cc.PhysicsSprite3D:create("Sprite3DTest/box.c3t", rbDes)
sprite:setTexture("Images/Icon.png")

--此方法  addChild 完后调用
sprite:syncNodeToPhysics() --同步节点转换为物理
sprite:setSyncFlag(cc.Physics3DComponent.PhysicsSyncFlag.PHYSICS_TO_NODE) --cc.Physics3DComponent.PhysicsSyncFlag.NONE 静态

local rigidBody = sprite:getPhysicsObj()
rigidBody:setLinearFactor(cc.vec3(1.0, 1.0, 1.0))
rigidBody:setLinearVelocity(linearVec)   --mass不为0
rigidBody:setAngularVelocity(cc.vec3(0.0, 0.0, 0.0))
rigidBody:setCcdMotionThreshold(0.5)    --连续碰撞发生的速度阈值
rigidBody:setCcdSweptSphereRadius(0.4)  --连续碰撞检测算法时扫过的球半径

--rigidBody:setKinematic(true) --运动

-- local rigidBody = cc.Physics3DRigidBody:create(rbDes)
-- local component = cc.Physics3DComponent:create(rigidBody)
-- local sprite = cc.Sprite3D:create("Sprite3DTest/boss.c3b")
-- sprite:addComponent(component)