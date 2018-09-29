//////////////////////////////////////////////////////////////spin
//opts 可从网站在线制作  code source http://fgnass.github.io/spin.js/
var opts = {
    lines: 13, // 花瓣数目
    length: 10, // 花瓣长度
    width: 10, // 花瓣宽度
    radius: 20, // 花瓣距中心半径
    corners: 1, // 花瓣圆滑度 (0-1)
    rotate: 0, // 花瓣旋转角度
    direction: 1, // 花瓣旋转方向 1: 顺时针, -1: 逆时针
    color: 'rgb(255,102,0)', // 花瓣颜色    default #5882FA
    speed: 1, // 花瓣旋转速度
    trail: 60, // 花瓣旋转时的拖影(百分比)
    shadow: false, // 花瓣是否显示阴影
    hwaccel: false, //spinner 是否启用硬件加速及高速旋转
    className: 'spinner', // spinner css 样式名称
    zIndex: 2e9, // spinner的z轴 (默认是2000000000)
    top: 'auto', // spinner 相对父容器Top定位 单位 px   defautl:auto
    left: '50%'// spinner 相对父容器Left定位 单位 px
};
var target = document.getElementById('divMySpin');
//var spinner = new Spinner(opts).spin(target);
var spinner = new Spinner(opts);
////////////////////////////////////////////////////////////////////////////////////
function mySpin() {//异步请求时spinner出现

    $("#divMySpin").text("");
    var target = $("#divMySpin").get(0);
    spinner.spin(target);
}