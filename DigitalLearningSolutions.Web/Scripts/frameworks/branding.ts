var bs = <HTMLSelectElement>document.getElementById('brand-select');
if (bs != null) {
  bs.addEventListener('change', function () {
    var style = this.value == '0' ? 'block' : 'none';
    var tbd = <HTMLElement>document.getElementById('new-brand-field');
    tbd.style.display = style;
    var tb = <HTMLInputElement>document.getElementById('brand-field');
    if (style == 'block') {
      tb.value = '';
      tb.required = true;
      tb.focus();
    }
    else {
      tb.required = false;
    }
  });
}
var cs = <HTMLSelectElement>document.getElementById('category-select');
if (cs != null) {
  cs.addEventListener('change', function () {
    var style = this.value == '0' ? 'block' : 'none';
    var tbd = <HTMLElement>document.getElementById('new-category-field');
    tbd.style.display = style;
    var tb = <HTMLInputElement>document.getElementById('category-field');
    if (style == 'block') {
      tb.value = '';
      tb.required = true;
      tb.focus();
    }
    else {
      tb.required = false;
    }
  });
}
var ts = <HTMLSelectElement>document.getElementById('topic-select');
if (ts != null) {
  ts.addEventListener('change', function () {
    var style = this.value == '0' ? 'block' : 'none';
    var tbd = <HTMLElement>document.getElementById('new-topic-field');
    tbd.style.display = style;
    var tb = <HTMLInputElement>document.getElementById('topic-field');
    if (style == 'block') {
      tb.value = '';
      tb.required = true;
      tb.focus();
    }
    else {
      tb.required = false;
    }
  });
}
