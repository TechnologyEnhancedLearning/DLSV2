const bs = <HTMLSelectElement>document.getElementById('brand-select');
if (bs != null) {
  bs.addEventListener('change', brandChanged);
}
function brandChanged() {
  const style = bs.value === '0' ? 'block' : 'none';
  const tbd = <HTMLElement>document.getElementById('new-brand-field');
  const tb = <HTMLInputElement>document.getElementById('brand-field');
  tbd.style.display = style;
  if (style === 'block') {
    tb.value = '';
    tb.required = true;
    tb.focus();
  } else {
    tb.required = false;
  }
}
const cs = <HTMLSelectElement>document.getElementById('category-select');
if (cs != null) {
  cs.addEventListener('change', categoryChanged);
}
function categoryChanged() {
  const style = cs.value === '0' ? 'block' : 'none';
  const tbd = <HTMLElement>document.getElementById('new-category-field');
  const tb = <HTMLInputElement>document.getElementById('category-field');
  tbd.style.display = style;
  if (style === 'block') {
    tb.value = '';
    tb.required = true;
    tb.focus();
  } else {
    tb.required = false;
  }
}
const ts = <HTMLSelectElement>document.getElementById('topic-select');
if (ts != null) {
  ts.addEventListener('change', topicChanged);
}
function topicChanged() {
  const style = ts.value === '0' ? 'block' : 'none';
  const tbd = <HTMLElement>document.getElementById('new-topic-field');
  const tb = <HTMLInputElement>document.getElementById('topic-field');
  tbd.style.display = style;
  if (style === 'block') {
    tb.value = '';
    tb.required = true;
    tb.focus();
  } else {
    tb.required = false;
  }
}
