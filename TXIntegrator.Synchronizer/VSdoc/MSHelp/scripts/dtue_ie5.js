// VERSION 7.1.2058
// This script works with IE 5.x+. Optimized for IE6.x

//************************** LOCALIZATION VARIABLES ***************************
// Load xml resource
jsPath = "MSHelp/scripts/";
//jsPath = "";
try {
	var xmldoc = new ActiveXObject("MSXML.DOMDocument");
	xmldoc.async = false;
	xmldoc.load(jsPath + "dtue.xml");
} catch (e) {}

// Variables for Feedback links
//var L_FeedbackLink_TEXT = getXMLText('L_FeedbackLink_TEXT');
var L_FeedbackLink_TEXT = "Send feedback";
//var L_MessageLink_TEXT = getXMLText('L_MessageLink_TEXT');
var L_MessageLink_TEXT ="";
//var L_MailToLink_TEXT = getXMLText('L_MailToLink_TEXT');
var L_MailToLink_TEXT = "Send feedback";

// Variables for the running head buttons
//var L_SeeAlso_TEXT = getXMLText('L_SeeAlso_TEXT');
//var L_Requirements_TEXT = getXMLText('L_Requirements_TEXT');
//var L_QuickInfo_TEXT = getXMLText('L_QuickInfo_TEXT');
//var L_FilterTip_TEXT = getXMLText('L_FilterTip_TEXT');		// tooltip for language button
//var L_Language_TEXT = getXMLText('L_Language_TEXT');		// heading for menu of programming languages
//var L_ShowAll_TEXT = getXMLText('L_ShowAll_TEXT');		// label for 'show all languages' menu item
var L_SeeAlso_TEXT = "See Also";
var L_Requirements_TEXT = "Requirements";
var L_QuickInfo_TEXT = "QuickInfo";
var L_FilterTip_TEXT = "Language Filter";		// tooltip for language button
var L_Language_TEXT = "Language";		// heading for menu of programming languages
var L_ShowAll_TEXT = "Show All";		// label for 'show all languages' menu item


// Variables for Expand-Collapse functions
//var L_ExpandAll_TEXT = getXMLText('L_ExpandAll_TEXT');
//var L_CollapseAll_TEXT = getXMLText('L_CollapseAll_TEXT');
//var L_ExColl_TEXT = getXMLText('L_ExColl_TEXT');
var L_ExpandAll_TEXT = "Expand All";
var L_CollapseAll_TEXT = "Collapse All";
var L_ExColl_TEXT = "Click to Expand or Collapse";

// Localization Variables for NavFailPage Param
var L_Keyword_TEXT = "Keyword =";
var L_IndexMoniker_TEXT = "IndexMoniker =";
var L_URL_TEXT = "Source URL =";

// defines the running head popup box. Localizable
var L_PopUpBoxStyle_Style = "WIDTH:200PX; PADDING:5px 7px 7px 7px; BACKGROUND-COLOR:#FFFFCC; BORDER: 1px SOLID #999999; VISIBILITY:HIDDEN; POSITION:ABSOLUTE; TOP:0PX; LEFT:0PX; Z-INDEX:2;";

//*************************** END LOCALIZATION ********************************

//**** Do not localize the following lines, they allow for bilingual files ****
var US_See_Also = "See Also";
var US_Requirements = "Requirements";
var US_QuickInfo = "QuickInfo"

//***ScriptSettings
var bRefTips = true		//Show RefTips
var bInCHM = false		//CHM check
var popOpen, theImg, theDiv, e;
var advanced = false;
var curLang = null;
var showAll = true;
var cook = null;
var scrollPos = 0;
var baseUrl = jsPath; //jsPath comes from the dtuelink.js
var popupDIV = "<DIV ID='popUpWindow' STYLE='"+L_PopUpBoxStyle_Style+"'>" + "</DIV>";

//***************************** END VARIABLES *********************************


//**************** IE-only stuff port for other browsers ***********************

// add .sourceIndex property to Node object if not present
if (window.Node && Node.prototype && !Node.prototype.sourceIndex) {
	Node.prototype.__defineGetter__("sourceIndex", function () {
		return getSourceIndex(this);
  } )
}
function getSourceIndex(element) {
	var all = document.getElementsByTagName("*");
	for (var i = 0; i < all.length; i++) {
		if (all[i] == element) {
			return i;
		}
	} 
	return -1;
}


// add .outerHTML property to Node object if not present
if (window.HTMLElement && HTMLElement.prototype && !HTMLElement.prototype.outerHTML) {
	HTMLElement.prototype.__defineGetter__("outerHTML",  function () {
		return getOuterHTML(this);
  } )
  
	HTMLElement.prototype.__defineSetter__("outerHTML", function (sHTML) {
		var r = this.ownerDocument.createRange();
		r.setStartBefore(this);
		var df = r.createContextualFragment(sHTML);
		this.parentNode.replaceChild(df, this);
	})  
}
function getOuterHTML(node){
	var clone = node.cloneNode(true)
	var container = document.createElement("div");
	container.appendChild(clone)
	return container.innerHTML
}


// add .contains() method to Node object if not present
if (window.Node && Node.prototype && !Node.prototype.contains)
{
	Node.prototype.contains = function (arg) {
		return !!(this.compareDocumentPosition(arg) & 16)
	}
}


// add .insertAdjacentHTML() method if not present
if (window.HTMLElement && HTMLElement.prototype && !HTMLElement.prototype.insertAdjacentHTML) {
	HTMLElement.prototype.insertAdjacentHTML = function (sWhere, sHTML) {
	   var df;   // : DocumentFragment
	   var r = this.ownerDocument.createRange();
	   
	   switch (String(sWhere).toLowerCase()) {  // convert to string and unify case
	      case "beforebegin":
	         r.setStartBefore(this);
	         df = r.createContextualFragment(sHTML);
	         this.parentNode.insertBefore(df, this);
	         break;
	         
	      case "afterbegin":
	         r.selectNodeContents(this);
	         r.collapse(true);
	         df = r.createContextualFragment(sHTML);
	         this.insertBefore(df, this.firstChild);
	         break;
	         
	      case "beforeend":
	         r.selectNodeContents(this);
	         r.collapse(false);
	         df = r.createContextualFragment(sHTML);
	         this.appendChild(df);
	         break;
	         
	      case "afterend":
	         r.setStartAfter(this);
	         df = r.createContextualFragment(sHTML);
	         this.parentNode.insertBefore(df, this.nextSibling);
	         break;
	   }   
	}
}

// ****************************************************************************
// *                           Common code                                    *
// ****************************************************************************


function getXMLText(term) {
	var out="";
	//try {
		out = xmldoc.selectSingleNode("/UI/String[@Id='" + term + "']").text;		
	//}
	//catch (ex){}
	return out;
}

// Check for <meta Name="RefTips" Content="False">
if (bRefTips==true){
	var mColl = document.getElementsByTagName("META");
	for (i=0; i<mColl.length; i++){
		if (mColl[i].name.toUpperCase()=="REFTIPS"){
			if (mColl[i].content.toUpperCase()=="FALSE") bRefTips = false;
		}
	}
}

// Check for <META Name="InChm" Content="True">
if (bInCHM==false){
	var mColl = document.getElementsByTagName("META");
	for (i=0; i<mColl.length; i++){
		if (mColl[i].name.toUpperCase()=="INCHM"){
			if (mColl[i].content.toUpperCase()=="TRUE") bInCHM = true;
		}
	}
}

if (ieVer >= 5) {
	var advanced = true;
}

if (advanced) {
	window.onload = bodyOnLoad;
	if (bInCHM==false){
		window.onbeforeprint = set_to_print;
		window.onafterprint = reset_form;
	}
}

function finishOnLoad(){
	document.onkeypress = ieKey;
	window.onresize = closeIE4;
	document.body.onclick = bodyOnClick;
	//IF THE USER HAS IE4+ THEY WILL BE ABLE TO VIEW POPUP BOXES
	if (advanced){
		document.body.insertAdjacentHTML('beforeEnd', popupDIV);
	}
return;
}


function bodyOnClick(e){
	if (advanced) {
		try {
			if (!e) var e = window.event;
			var elem = (e.target) ? e.target : e.srcElement;
			for (; elem; elem = elem.parentNode) {
				if (elem.id == "reftip")
					return;
			}
			hideTip();
			closeMenu();
			hideSeeAlso();
		} catch (ex) {}
		resizeBan();
	}
}


function ieKey(e){
	if (!e) var e = window.event;
	if (e.keyCode == 27){
		try {
			hideTip();
			closeMenu();
			hideSeeAlso();
		} catch (ex) {}
		resizeBan();
		closeIE4();
	}
return;
}


function closeIE4(){
	try {
		document.getElementById("popUpWindow").style.visibility = "hidden";
		popOpen = false;
	} catch (ex) {}
	resizeBan();  //also resize the non-scrolling banner
return;
}


function bodyOnLoad(){
	try {
		if (advanced) {
			var address = location.href;
			var bookmarkStart = address.indexOf("#")
			// If it has a bookmark, check to see if Language is near A Name
			if (bookmarkStart>-1){
				var bookmark = address.substring(bookmarkStart+1);
				curLang = findLanguage(bookmark);
			}
			else {
				// Check the dynamic help window for current language.
				var cLang;
				try{
					for (i=1; i< window.external.ContextAttributes.Count; i++){
						if(window.external.ContextAttributes(i).Name.toUpperCase()=="DEVLANG"){
							var b = window.external.ContextAttributes(i).Values.toArray();
							cLang = b[0].toUpperCase();
						}
					}
				}
				catch(e){}
				if (cLang != null){
					if (cLang.indexOf("VB")!=-1) curLang = "Visual Basic";
					if (cLang.indexOf("VC")!=-1) curLang = "C++";
					if (cLang.indexOf("CSHARP")!=-1) curLang = "C#";
				}
	
				if (curLang == null){
					var l = "";
					var multipleLang = false;
					// Check to see what the help filter is set to.
					try {l = window.external.Help.FilterQuery.toUpperCase();}
					catch(e){}
					if (l.indexOf("VISUAL BASIC")!=-1){
						cLang = "Visual Basic";
						}
					if (l.indexOf("VISUAL C++")!=-1){
						if (cLang!=null) multipleLang = true;
						cLang = "C++";
						}
					if (l.indexOf("C#")!=-1){
						if (cLang!=null) multipleLang = true;
						cLang = "C#";
						}
					if (multipleLang==false) curLang = cLang;
					
				}
			}
			if (curLang != null)
				showAll = false;
			initLangs();
			resizeBan();
			if (bRefTips==true) initReftips();
			initSeeAlso();
		}
	} catch (exx){}
finishOnLoad();
//set the scroll position
try{document.getElementById("nstext").scrollTop = scrollPos;}
catch(e){}
}


function findLanguage(bookmark){
// Find span associated with bookmark
	var found = false
	var aColl = document.getElementsByTagName("A");
	for (i=0; i<aColl.length; i++){
		if (aColl(i).name.toUpperCase()==bookmark.toUpperCase()){
			var elem = null
			for(t = 1; t<4; t++){
				elem = document.getElementsByTagName("*")[aColl(i).sourceIndex + t];
				if (elem.tagName.toUpperCase()=="SPAN")
					found = true;
					break;
			}
			break;
		}
	}
//if found, filter language
	if (found){
		var lang = elem.firstChild.nodeValue
		return lang.substring(lang.indexOf("[") + 1, lang.indexOf("]"));
	}
}


// ****************************************************************************
// *                        Language filtering                                *
// ****************************************************************************

function initLangs(){
	try {
		var hdr = document.getElementById("hdr");
		if (!hdr)
			return;
	
		var langs = new Array;
		var spans = document.getElementsByTagName("SPAN");
		if (spans) {
			var iElem = spans.length;
			for (iElem = 0; iElem < spans.length; iElem++) {
				var elem = spans[iElem];
				if (elem.className == "lang") {
	
					// Update the array of unique language names.
					var a = elem.firstChild.nodeValue.split(",");
					for (var iTok = 0; iTok < a.length; iTok++) {
						if (a[iTok]=="[A]"){
							langs[0]="A";
							elem.parentNode.outerText="";
						}
						var m = a[iTok].match(/([A-Za-z].*[A-Za-z+#0-9])/);
						if (m) {
							var iLang = 0;
							while (iLang < langs.length && langs[iLang] < m[1])
								iLang++;
							if (iLang == langs.length || langs[iLang] != m[1]) {
								var before = langs.slice(0,iLang);
								var after = langs.slice(iLang);
								langs = before.concat(m[1]).concat(after);
							}
						}
					}
				}
			}
		}
	
		if (langs.length > 0) {
			var pres = document.getElementsByTagName("PRE");
			if (pres) {
				for (var iPre = 0; iPre < pres.length; iPre++)
					initPreElem(pres[iPre]);
			}
	
			if (curLang == null){
				var obj = document.getElementById("obj_cook");
				if (obj && obj.object) {
					cook = obj;
					if (obj.getValue("lang.all") != "1") {
						var lang = obj.getValue("lang");
						var c = langs.length;
						for (var i = 0; i != c; ++i) {
							if (langs[i] == lang) {
								curLang = langs[i];
								showAll = false;
							}
						}
					}
				}
	
			}
	
			var iLim = document.body.getElementsByTagName("*").length;
			var head = null;
			for (var i = 0; i < iLim; i++) {
				var elem = document.body.getElementsByTagName("*")[i];
				if (elem.tagName.match(/^(P)|(PRE)|([DOU]L)$/))
					break;
				if (elem.tagName.match(/^H[1-6]$/)) {
					head = elem;
					head.insertAdjacentHTML('beforeEnd', '<SPAN CLASS=ilang></SPAN>');
				}
			}
	
			iLang = 0;
			foundA = false;
			while (iLang != langs.length){
				if (langs[iLang]=="A")
					foundA = true;
				iLang++;
			}
			if (!foundA)
				// don't language button on
				var td = hdr.insertCell(0);
			if (td) {
				// Add the language button to the button bar.
				td.className = "button1";
				td.onkeyup = ieKey;
				td.onkeypress = langMenu;
				td.onclick = langMenu;
				td.innerHTML = '<IMG id="button1" name="button1" SRC="' + baseUrl + 'Filter1a.gif' + '" ALT="' +
					L_FilterTip_TEXT + '" title="' + L_FilterTip_TEXT + '" BORDER=0 TABINDEX=0>';
	
				// Add the menu.
				var div = '<DIV ID="lang_menu" CLASS=langMenu onkeypress=ieKey><B>' + L_Language_TEXT + '</B><UL>';
				for (var i = 0; i < langs.length; i++)
					div += '<LI><A HREF="" ONCLICK="chooseLang(event, this)">' + langs[i] + '</A><BR>';
				div += '<LI><A HREF="" ONCLICK="chooseAll()">' + L_ShowAll_TEXT + '</A></UL></DIV>';
				try{document.getElementById("nsbanner").insertAdjacentHTML('afterEnd', div);}
				catch(e){try{scrbanner.insertAdjacentHTML('afterEnd', div);}catch(e){}}
			}
	
			if (!showAll)
				filterLang();
		}
	} catch (exx) {}
}


function initPreElem(pre){
	var htm0 = pre.outerHTML;
	var reLang = /<span\b[^>]*class="?lang"?[^>]*>/i;
	var iFirst = -1;
	var iSecond = -1;

	iFirst = htm0.search(reLang);
	if (iFirst >= 0) {
		iPos = iFirst + 17;
		iMatch = htm0.substr(iPos).search(reLang);
		if (iMatch >= 0)
			iSecond = iPos + iMatch;
	}

	if (iSecond < 0) {
		var htm1 = trimPreElem(htm0);
		if (htm1 != htm0) {
			pre.insertAdjacentHTML('afterEnd', htm1);
			pre.outerHTML = "";
		}
	}
	else {
		var rePairs = /<(\w+)\b[^>]*><\/\1>/gi;

		var substr1 = htm0.substring(0,iSecond);
		var tags1 = substr1.replace(/>[^<>]+(<|$)/g, ">$1");
		var open1 = tags1.replace(rePairs, "");
		open1 = open1.replace(rePairs, "");

		var substr2 = htm0.substring(iSecond);
		var tags2 = substr2.replace(/>[^<>]+</g, "><");
		var open2 = tags2.replace(rePairs, "");
		open2 = open2.replace(rePairs, "");

		pre.insertAdjacentHTML('afterEnd', open1 + substr2);
		pre.insertAdjacentHTML('afterEnd', trimPreElem(substr1 + open2));
		pre.outerHTML = "";
	}	
}


function trimPreElem(htm){
	return htm.replace(/[ \r\n]*((<\/[BI]>)*)[ \r\n]*<\/PRE>/g, "$1</PRE>").replace(
		/\w*<\/SPAN>\w*((<[BI]>)*)\r\n/g, "\r\n</SPAN>$1"
		);
}


function getBlock(elem){
	while (elem && elem.tagName.match(/^([BIUA]|(SPAN)|(CODE)|(TD))$/))
		elem = elem.parentNode;
	return elem;
}


function langMenu(e){
	bodyOnClick();
	if (!e) var e = window.event;
	var btn = (e.target) ? e.target : e.srcElement
	if (btn.id=="button1"){
	btn.src = btn.src.replace("a.gif", "c.gif");}

	//ie
	try {
		e.returnValue = false;
		e.cancelBubble = true;
	} catch (ex) {}
	//mozilla
	try {
		e.preventDefault();
		e.stopPropagation();
	} catch (ex) {}

	var div = document.getElementById("lang_menu");
	var lnk = (e.target) ? e.target : e.srcElement;
	if (div && lnk) {
		var x = lnk.offsetLeft + lnk.offsetWidth - div.offsetWidth;
		div.style.pixelLeft = (x < 0) ? 0 : x;
		div.style.pixelTop = lnk.offsetTop + lnk.offsetHeight;
		div.style.visibility = "visible";
	}
}


function chooseLang(e, item){
	//ie
	try {
		window.event.returnValue = false;
		window.event.cancelBubble = true;
	} catch (ex) {}
	//mozilla
	try {
		e.preventDefault();
		e.stopPropagation();
	} catch (ex) {}
	

	if (item) {
		closeMenu();
		curLang = item.firstChild.nodeValue;
		showAll = false;
	}

	if (cook) {
		cook.putValue('lang', curLang);
		cook.putValue('lang.all', '');
	}

	filterLang();
}


function chooseAll(e){
	//ie
	try {
		window.event.returnValue = false;
		window.event.cancelBubble = true;
	} catch (ex) {}
	//mozilla
	try {
		e.preventDefault();
		e.stopPropagation();
	} catch (ex) {}

	closeMenu();

	showAll = true;
	if (cook)
		cook.putValue('lang.all', '1');

	unfilterLang();
}


function closeMenu(){
	var div = document.getElementById("lang_menu");
	if (div && div.style.visibility != "hidden") {
		var lnk = document.activeElement;
		if (lnk && lnk.tagName == "A")
			lnk.blur();

		div.style.visibility = "hidden";
	}
}


function getNext(elem){
	for (var i = elem.sourceIndex + 1; i < document.getElementsByTagName('*').length; i++) {
		var next = document.getElementsByTagName('*')[i];
		if (!elem.contains(next))
			return next;
	}
	return null;
}


function filterMatch(text, name){
	var a = text.split(",");
	for (var iTok = 0; iTok < a.length; iTok++) {
		var m = a[iTok].match(/([A-Za-z].*[A-Za-z+#0-9])/);
		if (m && m[1] == name)
			return true;
	}
	return false;
}


function topicHeading(head){
	try{var iLim = nstext.children.length;
	Section = nstext;}
	catch(e){try{var iLim = scrtext.children.length;
		Section = scrtext;}
		catch(e){var iLim = document.body.children.length;
		Section = document.body;
		}
	}
	var idxLim = head.sourceIndex;
	for (var i = 0; i < iLim; i++) {
		var elem = Section.children[i];
		if (elem.sourceIndex < idxLim) {
			if (elem.tagName.match(/^(P)|(PRE)|([DOU]L)$/))
				return false;
		}
		else
			break;
	}
	return true;
}


function filterLang(){
	var spans = document.getElementsByTagName("SPAN");
	for (var i = 0; i < spans.length; i++) {
		var elem = spans[i];
		if (elem.className == "lang") {
			var newVal = filterMatch(elem.firstChild.nodeValue, curLang) ? "block" : "none";
			var block = getBlock(elem);
			block.style.display = newVal;
			elem.style.display = "none";

			if (block.tagName == "DT") {
				var next = getNext(block);
				if (next && next.tagName == "DD")
					next.style.display = newVal;
			}
			else if (block.tagName == "DIV") {
				block.className = "filtered2";
			}
			else if (block.tagName.match(/^H[1-6]$/)) {
				if (topicHeading(block)) {
					if (newVal != "none") {
						var tag = null;
						if (block.children && block.children.length) {
							tag = block.children[block.children.length - 1];
							if (tag.className == "ilang") {
								tag.innerHTML = (newVal == "block") ?
									'&nbsp; [' + curLang + ']' : "";
							}
						}
					}
				}
				else {
					var next = getNext(block);
					while (next && !next.tagName.match(/^(H[1-6])$/)) {
						if (next.tagName =="DIV"){
							if (next.className.toUpperCase() != "TABLEDIV") break;
						}
						next.style.display = newVal;
						next = getNext(next);
					}
				}
			}
		}
		else if (elem.className == "ilang") {
			var block = getBlock(elem);
			if (block.tagName == "H1")
				elem.innerHTML = '&nbsp; [' + curLang + ']';
		}
	}
	resizeBan();
}


function unfilterLang(name){
	var spans = document.getElementsByTagName("SPAN");
	for (var i = 0; i < spans.length; i++) {
		var elem = spans[i];
		if (elem.className == "lang") {
			var block = getBlock(elem);
			block.style.display = "block";
			elem.style.display = "inline";

			if (block.tagName == "DT") {
				var next = getNext(block);
				if (next && next.tagName == "DD")
					next.style.display = "block";
			}
			else if (block.tagName == "DIV") {
				block.className = "filtered";
			}
			else if (block.tagName.match(/^H[1-6]$/)) {
				if (topicHeading(block)) {
					var tag = null;
					if (block.children && block.children.length) {
						tag = block.children[block.children.length - 1];
						if (tag && tag.className == "ilang")
							tag.innerHTML = "";
					}
				}
				else {
					var next = getNext(block);
					while (next && !next.tagName.match(/^(H[1-6])$/)) {
						if (next.tagName =="DIV"){
							if (next.className.toUpperCase() != "TABLEDIV") break;
						}
						next.style.display = "block";
						next = getNext(next);
					}
				}
			}
		}
		else if (elem.className == "ilang") {
			elem.innerHTML = "";
		}
	}
	resizeBan();
}


// ****************************************************************************
// *                      Reftips (parameter popups)                          *
// ****************************************************************************

function initReftips(){
	var DLs = document.getElementsByTagName("DL");
	var PREs = document.getElementsByTagName("PRE");
	if (DLs && PREs) {
		var iDL = 0;
		var iPRE = 0;
		var iSyntax = -1;
		for (var iPRE = 0; iPRE < PREs.length; iPRE++) {
			if (PREs[iPRE].className == "syntax") {
				while (iDL < DLs.length && DLs[iDL].sourceIndex < PREs[iPRE].sourceIndex)
					iDL++;			
				if (iDL < DLs.length) {
					initSyntax(PREs[iPRE], DLs[iDL]);
					iSyntax = iPRE;
				}
				else
					break;
			}
		}

		if (iSyntax >= 0) {
			var last = PREs[iSyntax];
			if (last.parentNode.tagName == "DIV") last = last.parentNode;						last.insertAdjacentHTML('afterEnd','<DIV ID=reftip CLASS=reftip STYLE="position:absolute;visibility:hidden;overflow:visible;"></DIV>');
		}
	}
}


function initSyntax(pre, dl){
	try {
		var strSyn = pre.outerHTML;
		var ichStart = strSyn.indexOf('>', 0) + 1;
		var terms = dl.children.tags("DT");
		if (terms) {
			for (var iTerm = 0; iTerm < terms.length; iTerm++) {
				if (terms[iTerm].innerHTML.indexOf("<!--join-->")!=-1){
					var word = terms[iTerm].firstChild.nodeValue.replace(/\s$/, "");
					var ichMatch = findTerm(strSyn, ichStart, word);
					if (ichMatch < 1){
						word = word.replace(/\s/, "&nbsp;")
						ichMatch = findTerm(strSyn, ichStart, word);
					}
					while (ichMatch > 0) {
						var strTag = '<A HREF="" ONCLICK="showTip(event, this)" CLASS="synParam">' + word + '</A>';
	
						strSyn =
							strSyn.slice(0, ichMatch) +
							strTag +
							strSyn.slice(ichMatch + word.length);
						ichMatch = findTerm(strSyn, ichMatch + strTag.length, word);
					}
					
				}
			}
			for (var iTerm = 0; iTerm < terms.length; iTerm++) {
				if (terms[iTerm].innerHTML.indexOf("<!--join-->")==-1){
				var words = terms[iTerm].firstChild.nodeValue.replace(/\[.+\]/g, " ").replace(/,/g, " ").split(" ");
					var htm = terms[iTerm].innerHTML;
					for (var iWord = 0; iWord < words.length; iWord++) {
						var word = words[iWord];
	
						if (word.length > 0 && htm.indexOf(word, 0) < 0)
							word = word.replace(/:.+/, "");
	
						if (word.length > 0) {
							var ichMatch = findTerm(strSyn, ichStart, word);
							while (ichMatch > 0) {
								if (!isLinkText(strSyn.substring(ichMatch))){
									var strTag = '<A HREF="" ONCLICK="showTip(event, this)" CLASS="synParam">' + word + '</A>';
	
									strSyn =
										strSyn.slice(0, ichMatch) +
										strTag +
										strSyn.slice(ichMatch + word.length);
	
									ichMatch = findTerm(strSyn, ichMatch + strTag.length, word);
								}
								else ichMatch = findTerm(strSyn, ichMatch + word.length, word);
							}
						}
					}
				}
			}
		}
	
		// Replace the syntax block with our modified version.
		pre.outerHTML = strSyn;
	} catch (ex) {
	}
}


function findTerm(strSyn, ichPos, strTerm)
{
	var ichMatch = strSyn.indexOf(strTerm, ichPos);
	while (ichMatch >= 0) {
		var prev = (ichMatch == 0) ? '\0' : strSyn.charAt(ichMatch - 1);
		var next = strSyn.charAt(ichMatch + strTerm.length);
		if (!isalnum(prev) && !isalnum(next) && !isInTag(strSyn, ichMatch)) {
			var ichComment = strSyn.indexOf("/*", ichPos);
			while (ichComment >= 0) {
				if (ichComment > ichMatch) { 
					ichComment = -1;
					break; 
				}
				var ichEnd = strSyn.indexOf("*/", ichComment);
				if (ichEnd < 0 || ichEnd > ichMatch)
					break;
				ichComment = strSyn.indexOf("/*", ichEnd);
			}
			if (ichComment < 0) {
				ichComment = strSyn.indexOf("//", ichPos);
				var newPos = 0;
				if (ichComment >= 0) {
					while (isInTag(strSyn, ichComment)) { //checks to see if the comment is in a tag (and thus part of a URL)
						newPos = ichComment + 1;
						ichComment = strSyn.indexOf("//", newPos);
						if (ichComment < 0) 
							break;
					}
					while (ichComment >= 0) {
						if (ichComment > ichMatch) {
							ichComment = -1;
							break; 
						}
						var ichEnd = strSyn.indexOf("\n", ichComment);
						if (ichEnd < 0 || ichEnd > ichMatch)
							break;
						ichComment = strSyn.indexOf("//", ichEnd);
					}
				}
			}
			if (ichComment < 0)
				break;
		}
		ichMatch = strSyn.indexOf(strTerm, ichMatch + strTerm.length);
	}
	return ichMatch;
}
function isLinkText(strHtml){
	return strHtml.indexOf("<")==strHtml.toLowerCase().indexOf("<\/a>");
}

function isInTag(strHtml, ichPos)
{
	return strHtml.lastIndexOf('<', ichPos) >
		strHtml.lastIndexOf('>', ichPos);
}


function isalnum(ch){
	return ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || (ch == '_') || (ch == '-'));
}


function showTip(e, link){
	bodyOnClick();
	var tip = document.getElementById("reftip");
	if (!tip || !link)
		return;

	//ie
	try {
		window.event.returnValue = false;
		window.event.cancelBubble = true;
	} catch (ex) {}
	//mozilla
	try {
		e.preventDefault();
		e.stopPropagation();
	} catch (ex) {}
	

	// Hide the tip if necessary and initialize its size.
	tip.style.visibility = "hidden";
	tip.style.pixelWidth = 260;
	tip.style.pixelHeight = 24;

	// Find the link target.
	var term = null;
	var def = null;
	var DLs = document.getElementsByTagName("DL");
	for (var iDL = 0; iDL < DLs.length; iDL++) {
		if (DLs[iDL].sourceIndex > link.sourceIndex) {
			var dl = DLs[iDL];
			var iMax = dl.children.length - 1;
			for (var iElem = 0; iElem < iMax; iElem++) {
				var dt = dl.children[iElem];
				if (dt.tagName == "DT" && dt.style.display != "none") {
					if (findTerm(dt.firstChild.nodeValue, 0, link.firstChild.nodeValue) >= 0) {
						var dd = dl.children[iElem + 1];
						if (dd.tagName == "DD") {
							term = dt;
							def = dd;
						}
						break;
					}
				}
			}
			break;
		}
	}

	if (def) {
		window.linkElement = link;
		window.linkTarget = term;
		tip.innerHTML = '<DL><DT>' + term.innerHTML + '</DT><DD>' + def.innerHTML + '</DD></DL>';
		window.setTimeout("moveTip()", 0);
	}
}


function jumpParam(){
	hideTip();

	window.linkTarget.scrollIntoView();
	document.body.scrollLeft = 0;

	flash(3);
}


function flash(c){
	window.linkTarget.style.background = (c & 1) ? "#CCCCCC" : "";
	if (c)
		window.setTimeout("flash(" + (c-1) + ")", 200);
}


function moveTip(){
	var tip = document.getElementById("reftip");
	var link = window.linkElement;
	if (!tip || !link)
		return; //error

	var w = tip.offsetWidth;
	var h = tip.offsetHeight;

	if (w > tip.style.pixelWidth) {
		tip.style.pixelWidth = w;
		window.setTimeout("moveTip()", 0);
		return;
	}

	var maxw = document.body.clientWidth-20;
	var maxh = document.body.clientHeight - 200;

	if (h > maxh) {
		if (w < maxw) {
			w = w * 3 / 2;
			tip.style.pixelWidth = (w < maxw) ? w : maxw;
			window.setTimeout("moveTip()", 0);
			return;
		}
	}

	var x,y;

	var linkLeft = link.offsetLeft - document.body.scrollLeft;
	var linkRight = linkLeft + link.offsetWidth;

	try{var linkTop = link.offsetTop - nstext.scrollTop + nstext.offsetTop;}
	catch(e){var linkTop = link.offsetTop;}
	var linkBottom = linkTop + link.offsetHeight + 4;

	var cxMin = link.offsetWidth - 24;
	if (cxMin < 16)
		cxMin = 16;

	if ((linkLeft + cxMin + w <= maxw)&&(h+linkTop <= maxh + 150)) {
		x = linkLeft;
		y = linkBottom;
	}
	if ((linkLeft + cxMin + w <= maxw)&&(h+linkTop > maxh + 150)) {
		x = maxw - w;
		if (x > linkRight + 8)
			x = linkRight + 8;
		x = linkLeft;
		y = linkTop-h;
	}
	if ((linkLeft + cxMin + w >= maxw)&&(h+linkTop <= maxh + 150)) {
		x = linkRight - w;
		if (x < 0)
			x = 0;
		y=linkBottom;
	}
	if ((linkLeft + cxMin + w >= maxw)&&(h+linkTop > maxh + 150)) {
		x = linkRight - w;
		if (x < 0)
			x = 0;
		y = linkTop-h;
		if (y<0)
			y = 0;
	}
	link.style.background = "#CCCCCC";
	tip.style.pixelLeft = x + document.body.scrollLeft;
	tip.style.pixelTop = y;
	tip.style.visibility = "visible";
}


function hideTip(){
	if (window.linkElement) {
		window.linkElement.style.background = "";
		window.linkElement = null;
	}

	var tip = document.getElementById("reftip");
	if (tip) {
		tip.style.visibility = "hidden";
		tip.innerHTML = "";
	}
}


function beginsWith(s1, s2){
	// Does s1 begin with s2?
	return s1.toLowerCase().substring(0, s2.length) == s2.toLowerCase();
}


// ****************************************************************************
// *                           See Also popups                                *
// ****************************************************************************

function initSeeAlso(){
	var hdr = document.getElementById("hdr");
	if (!hdr)
		return;

	var divS = new String;
	var divR = new String;
	var heads = document.getElementsByTagName("H4");
	if (heads) {
		for (var i = 0; i < heads.length; i++) {
			var head = heads[i];
			var txt = head.firstChild.nodeValue;
			if (beginsWith(txt, L_SeeAlso_TEXT) || beginsWith(txt, US_See_Also)) {
				divS += head.outerHTML;
				var next = getNext(head);
				while (next && !next.tagName.match(/^(H[1-4])|(DIV)$/)) {
					divS += next.outerHTML;
					next = getNext(next);
				}
				while ((divS.indexOf("<MSHelp:ktable")!=-1)&&(divS.indexOf("<\/MSHelp:ktable>")!=-1)){
					divS=divS.substring(0, divS.indexOf("<MSHelp:ktable")) + divS.substring(divS.indexOf("<\/MSHelp:ktable>")+16);
				}
			}
			else if (beginsWith(txt, L_Requirements_TEXT) || beginsWith(txt, US_Requirements) || beginsWith(txt, L_QuickInfo_TEXT) || beginsWith(txt, US_QuickInfo)) {
				divR += head.outerHTML;
				var next = getNext(head);
				var isValid = true;
				while (isValid){
					if (next && !next.tagName.match(/^(H[1-4])$/)){
						if (next.tagName == "DIV" && next.outerHTML.indexOf("tablediv")==-1)
								isValid = false;
						if (isValid){
							divR += next.outerHTML;
							next = getNext(next);
						}
					}
					else
						isValid = false;
				}
			}
		}
	}

	var pos = getNext(hdr.parentNode);
	if (pos) {
		if (divR != "") {
			divR = '<DIV ID=rpop CLASS=sapop onkeypress=ieKey>' + divR + '</DIV>';
			var td = hdr.insertCell(0);
			if (td) {
				td.className = "button1";
				td.onclick = showRequirements;
				td.onkeyup = ieKey;
				td.onkeypress = showRequirements;
				td.innerHTML = '<IMG id=button1 name="button1" SRC="' + baseUrl + 'Requirements1a.gif' + '" ALT="' + L_Requirements_TEXT + '" title="' + L_Requirements_TEXT + '" BORDER=0 TABINDEX=0>';
				if (advanced)
					try{document.getElementById("nsbanner").insertAdjacentHTML('afterEnd', divR);}
					catch(e){try{scrbanner.insertAdjacentHTML('afterEnd', divR);}catch(e){}}
				else
					document.body.insertAdjacentHTML('beforeEnd', divR);
			}
		}
		if (divS != "") {
			divS = '<DIV ID=sapop CLASS=sapop onkeypress=ieKey>' + divS + '</DIV>';
			var td = hdr.insertCell(0);
			if (td) {
				td.className = "button1";
				td.onclick = showSeeAlso;
				td.onkeyup = ieKey;
				td.onkeypress = showSeeAlso;
				td.innerHTML = '<IMG id=button1 name="button1" SRC="' + baseUrl + 'SeeAlso1a.gif' + '" ALT="' + L_SeeAlso_TEXT + '" title="' + L_SeeAlso_TEXT + '" BORDER=0 TABINDEX=0>';
				if (advanced)
					try{document.getElementById("nsbanner").insertAdjacentHTML('afterEnd', divS);}
					catch(e){try{scrbanner.insertAdjacentHTML('afterEnd', divS);}catch(e){}}
				else
					document.body.insertAdjacentHTML('beforeEnd', divS);
			}
		}
	}
}

function resetButtons(){
	//unclick buttons...
	var btns = document.getElementsByName("button1");
	if (btns) {
		for (var i = 0; i < btns.length; i++){
			btns[i].src = btns[i].src.replace("c.gif", "a.gif");
		}
	}
}

function showSeeAlso(e){
	if (!e) var e = window.event;
	bodyOnClick();
	var btn = (e.target) ? e.target : e.srcElement
	if (btn.id=="button1"){
	btn.src = btn.src.replace("a.gif", "c.gif");}

	//ie
	try {
		e.returnValue = false;
		e.cancelBubble = true;
	} catch (ex) {}
	//mozilla
	try {
		e.preventDefault();
		e.stopPropagation();
	} catch (ex) {}

	var div = document.getElementById("sapop");
	var lnk = (e.target) ? e.target : e.srcElement;

	if (div && lnk) {
		div.style.pixelTop = lnk.offsetTop + lnk.offsetHeight;
		div.style.visibility = "visible";
	}
}


function showRequirements(e){
	if (!e) var e = window.event;
	bodyOnClick();
	var btn = (e.target) ? e.target : e.srcElement
	if (btn.id=="button1"){
	btn.src = btn.src.replace("a.gif", "c.gif");}

	//ie
	try {
		e.returnValue = false;
		e.cancelBubble = true;
	} catch (ex) {}
	//mozilla
	try {
		e.preventDefault();
		e.stopPropagation();
	} catch (ex) {}

	var div = document.getElementById("rpop");
	var lnk = (e.target) ? e.target : e.srcElement;

	if (div && lnk) {
		div.style.pixelTop = lnk.offsetTop + lnk.offsetHeight;
		div.style.visibility = "visible";
	}
}


function hideSeeAlso(){
	var div = document.getElementById("sapop");
	if (div)
		div.style.visibility = "hidden";

	var div = document.getElementById("rpop");
	if (div)
		div.style.visibility = "hidden";
}

function loadAll(){
	try {
		scrollPos = allHistory.getAttribute("Scroll");
	}
	catch(e){	}
}

function saveAll(){
	try{
		allHistory.setAttribute("Scroll",nstext.scrollTop);
	}
	catch(e){	}
}

// ****************************************************************************
// *                             Expand-Collapse                              *
// ****************************************************************************
/* keeping state requires the document to have the following tags... the meta tag may have to be before the body tag.
	<META NAME="save" CONTENT="history">
	<DIV id=sHistory class=saveHistory onsave="fnSave()" onload="fnLoad()"></DIV>
*/

/* required in the css
div.saveHistory
	{
	behavior:url(#default#savehistory);
	}
*/
function fnLoad(){
	var oArray = sHistory.getAttribute("sPersistState").split("|");
	if (oArray.length>0){
		var offset = oArray[0]-sHistory.sourceIndex;
		for(var i=1;i<oArray.length;i++){
			document.getElementsByTagName("*")[oArray[i]-offset].style.display="block";
			document.getElementsByTagName("*")[oArray[i]-(offset+2)].parentNode.getElementsByTagName('IMG')(0).src = baseUrl + "coc.gif";
		}
	document.getElementById("ExPandAll").innerHTML=sHistory.getAttribute("sPersistStateAll");
	}
}
function fnSave(){
	var sArray="";
	for(var i=0;i<document.getElementsByTagName("*").length;i++){
		oTrap=document.getElementsByTagName("*")[i];
		if((oTrap.tagName=="DIV")&&(oTrap.className=="expandBody")){
			if (oTrap.style.display == "block") {
				if(sArray.length==0){
					sArray=sHistory.sourceIndex + "|" + i;
				}
				else sArray+="|" + i;
			}
		}
	}
	sHistory.setAttribute("sPersistState",sArray);
	sHistory.setAttribute("sPersistStateAll", document.getElementById("ExPandAll").innerHTML);
}
function makeExpandable(title, level){
	if (title!="")document.write("<a href=\"\#\" onClick='callExpand()' id=\"ExPand\" Class=\"expandLink" + level + "\"><IMG CLASS=\"ExPand\" SRC=\"" + baseUrl + "coe.gif\" HEIGHT=9 WIDTH=9 ALT=\"" + L_ExColl_TEXT + "\" BORDER=0>&nbsp;" + title + "</a><BR><div CLASS=\"expandBody\">");
	else document.write("<a href=\"\#\" id=\"ExPandAll\" onClick='callExpandAll()' Class=\"expandLink" + level + "\"><IMG CLASS=\"ExPandAll\" SRC=\"" + baseUrl + "coe.gif\" HEIGHT=9 WIDTH=9 ALT=\"" + L_ExColl_TEXT + "\" BORDER=0>&nbsp;" + L_ExpandAll_TEXT + "</A>");
}

function getImage(){
	for (var a = 0; a < 7; a++){
      	if ((e.tagName != 'A') && (e.parentNode != null)){e = e.parentNode;}
		var elemImg = e;
		if(elemImg.tagName == 'A'){elemImg = e.getElementsByTagName('IMG')[0]; break;}}
return elemImg;}

function callExpand(ev){
	if (!ev) var ev = window.event;
	//DO EXPAND/COLLAPSE
	e = (ev.target) ? ev.target : ev.srcElement;
	//PREVENTS NAVIGATION ON HREF TAGS
	event.returnValue = false;

	//FIND THE EXPAND/COLLAPSE PORTION AND ASCERTAIN BLOCK VS NONE
	var theDiv = GrabtheExpandDiv(e);
		
	//THIS PART WRITES THE PROPER IMAGE BESIDE THE TEXT
	if (theDiv.style.display == 'block'){
		var theImg = getImage(e);
		theImg.src = baseUrl + "coe.gif";
		theDiv.style.display = "none";}
	else {
		var theImg = getImage(e);
		theImg.src = baseUrl + "coc.gif";
		theDiv.style.display = "block";}
return;}

function GrabtheExpandDiv(e){
//FIND AREA TO EXPAND/COLLAPSE
	var theExpandDiv;
	for (var a = 0; a < 7; a++){
    	var theTag = e.sourceIndex + e.children.length + a;
    	theExpandDiv= document.getElementsByTagName("*")[theTag];
     	if (((theExpandDiv.tagName == 'DIV') && ((theExpandDiv.className.toLowerCase().indexOf("expandbody")!=-1))) || theTag == document.getElementsByTagName("*").length){break;}}
return theExpandDiv;
}

function callExpandAll(ev){
	if (!ev) var ev = window.event;
	e = (ev.target) ? ev.target : ev.srcElement;
	//PREVENTS NAVIGATION ON HREF TAGS
	event.returnValue = false;
	if (e.tagName=="IMG") e = e.parentNode;
	//Expand or Collapse?
	if (e.innerHTML.indexOf(L_ExpandAll_TEXT) != -1){eOrC="block"}else{eOrC="none"}
	if (eOrC=="block"){
		e.innerHTML="<IMG CLASS='ExPand' SRC=\"" + baseUrl + "coc.gif\" HEIGHT='9' WIDTH='9' ALT='" + L_ExColl_TEXT + "' BORDER='0'>&nbsp;" + L_CollapseAll_TEXT;}
	else{
		e.innerHTML="<IMG CLASS='ExPand' SRC=\"" + baseUrl + "coe.gif\" HEIGHT='9' WIDTH='9' ALT='" + L_ExColl_TEXT + "' BORDER='0'>&nbsp;" + L_ExpandAll_TEXT;}
	for (var a = 0; a < document.getElementsByTagName("*").length; a++){ 
		e=document.getElementsByTagName("*")[a];

		if (e.id.indexOf('ExPand') != -1){

			if (e.id.indexOf('ExPandAll') == -1){

			var theDiv = GrabtheExpandDiv(e);
			if (eOrC == 'none'){
				theImg = getImage(e);
				theImg.src = baseUrl + "coe.gif";
				theDiv.style.display = eOrC;}
			else {
				theImg = getImage(e);
				theImg.src = baseUrl + "coc.gif";
				theDiv.style.display = eOrC;}
			}
		}
	}
return;}


// ****************************************************************************
// *                           Nonscrolling region                            *
// ****************************************************************************

function resizeBan(){
//resizes nonscrolling banner
	if (document.body.clientWidth==0) return;
	var oBanner= document.getElementById("nsbanner");
	var oText= document.getElementById("nstext");
	if (oText == null) return;
	var oBannerrow1 = document.getElementById("bannerrow1");
	var oTitleRow = document.getElementById("titlerow");
	if (oBannerrow1 != null){
		var iScrollWidth = document.getElementById("bodyID").scrollWidth;
		oBannerrow1.style.marginRight = 0 - iScrollWidth;
	}
	if (oTitleRow != null){
		oTitleRow.style.padding = "0px 10px 0px 22px; ";
	}
	if (oBanner != null){
		document.body.scroll = "no"
		oText.style.overflow= "auto";
 		oBanner.style.width= document.body.offsetWidth// - 2;
		oText.style.paddingRight = "20px"; // Width issue code
		//oText.style.width= document.body.offsetWidth - 4;
		oText.style.top=0;
		if (document.body.clientHeight > oBanner.offsetHeight)
    			oText.style.height = document.body.clientHeight - oBanner.offsetHeight;//oBanner.offsetHeight 
		else oText.style.height=0
	}	
	try{document.getElementById("nstext").setActive();} //allows scrolling from keyboard as soon as page is loaded. Only works in IE 5.5 and above.
	catch(e){}
	resetButtons();
} 


function set_to_print(){
//breaks out of divs to print

	var i;

	if (window.text)document.all.text.style.height = "auto";
			
	for (i=0; i < document.getElementsByTagName("*").length; i++){
		if (document.getElementsByTagName("*")[i].tagName == "BODY") {
			document.getElementsByTagName("*")[i].scroll = "yes";
			}
		if (document.getElementsByTagName("*")[i].id == "nsbanner") {
			document.getElementsByTagName("*")[i].style.margin = "0px 0px 0px 0px";
			document.getElementsByTagName("*")[i].style.width = "100%";
			}
		if (document.getElementsByTagName("*")[i].id == "nstext") {
			document.getElementsByTagName("*")[i].style.overflow = "visible";
			document.getElementsByTagName("*")[i].style.top = "5px";
			document.getElementsByTagName("*")[i].style.width = "100%";
			document.getElementsByTagName("*")[i].style.padding = "0px 10px 0px 30px";
			}
/*		if (document.all[i].tagName == "A") {
			document.all[i].outerHTML = "<A HREF=''>" + document.all[i].innerHTML + "</a>";
			}
*/
		}
}


function reset_form(){
//returns to the div nonscrolling region after print

	 document.location.reload();
}


// ****************************************************************************
// *                        Feedback & other footer links                     *
// ****************************************************************************

function sendfeedback(msdnid, alias){
	var url = location.href;
	var title = document.getElementsByTagName("TITLE")[0].firstChild.nodeValue;
	var browser = navigator.appName + " " + navigator.appVersion
	location.href = "mailto:"+alias+"?subject=" + msdnid + "%20" + title + "&body=Topic%20ID:%20" + msdnid + "%0d%0aTopic%20Title:%20" + title + "%0d%0aURL:%20" + url + "%0d%0aBrowser:%20" + browser + "%0d%0a%0d%0aComments:%20";
}

function writefeedbacklink(){
	//writes feedback link
	msdnid = arguments[0];
	contextid = arguments[1];
	topictitle = arguments[2];
	href = "http://beta.visualstudio.net/feedback.asp?feedback=doc&msdnid="+msdnid+"&contextid="+contextid+"&topictitle="+topictitle;
	document.writeln("<a href="+href+">"+L_FeedbackLink_TEXT+"</a>");
}


function writemessagelink(){
	//Writes jump to PSS web site redirector
	//code tbd
	//Use L_MessageLink_TEXT variable from Localization Variables located at top of script.
	msdnid = arguments[0];
	href = "http://www.microsoft.com/contentredirect.asp?prd=vs&pver=7.0&id="+msdnid;
	document.writeln("<a href="+href+">"+L_MessageLink_TEXT+"</a>");
}


function writemailtolink(){
	//writes feedback link
	emailalias = arguments[0];
	contextid = arguments[1];
	topictitle = arguments[2];

	href = "mailto:"+emailalias+"?subject=Feedback%20on%20topic%20-%20"+topictitle+",%20URL%20-%20"+contextid;
	document.writeln("<a href="+href+">"+L_MailToLink_TEXT+"</a>");
}


// ****************************************************************************
// *                       NavFailPage Parameter Insertion                    *
// ****************************************************************************

function navfailpageparam() {
	var keyString = "keywords=";
	var idxString = "index=";
	var urlString = "url=";
	var query = window.location.search;
	var j = 0;
	var delim;
	var i = query.indexOf(keyString);

	document.write("<p>" + L_Keyword_TEXT + " <span id=keywordsSpan></span><br>");
	document.write(L_IndexMoniker_TEXT + " <span id=indexSpan></span><br>");
	document.write(L_URL_TEXT + " <span id=urlSpan></span></p>");

	if (i >= 0) {
		i += keyString.length;
		delim = query.charAt(i);
		j = query.indexOf(delim, i+1);
		if (j > 0) keywordsSpan.firstChild.nodeValue = query.substring(i, j+1);
	}

	i = query.indexOf(idxString);
	if (i >= 0) {
		i += idxString.length;
		delim = query.charAt(i);
		j = query.indexOf(delim, i+1);
		if (j > 0) indexSpan.firstChild.nodeValue = query.substring(i, j+1);
	}

	i = query.indexOf(urlString);
	if (i >= 0) {
		i += urlString.length;
		delim = query.charAt(i);
		j = query.indexOf(delim, i+1);
		if (j > 0) urlSpan.firstChild.nodeValue = query.substring(i, j+1);
	}
}

