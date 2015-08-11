if(window.Element && Element.prototype.__defineGetter__){
	Element.prototype.__defineGetter__("currentStyle",function(){
        if (document.defaultView && document.defaultView.getComputedStyle) {
			return document.defaultView.getComputedStyle(this, "");
		}
		return {};
    });
}
function ExpandCollapse(e){
	if(e.nodeType==3)e=e.nextSibling;
	e.style.display = e.currentStyle.display=="none"?"":"none";
}
function ToggleSwitch(i){
	if(i.src.indexOf("collapse.")>0){
		i.src=i.src.replace("collapse","expand");
		i.title="展开";
	}else{
		i.src=i.src.replace("expand","collapse");
		i.title="折叠";
	}
}
function GetSelection(){
	if(window.getSelection){
		return getSelection();
	}
	if(document.selection){
		return document.selection.createRange().text;
	}
	return "";
}
window.onload = function(){
	var hash = window.location.hash;
	if(hash && (hash=hash.substr(1))){
        var e = document.getElementById(hash);
		if(e&&e.nextSibling)
			ExpandCollapse(e.nextSibling);
	}
}