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
    tb.setCustomValidity("");
    tb.focus();
  } else {
    tb.value = "";
    tb.setCustomValidity("");
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
    tb.setCustomValidity("");
    tb.focus();
  } else {
    tb.value = "";
    tb.setCustomValidity("");
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
    tb.setCustomValidity("");
    tb.focus();
  } else {
    tb.value = "";
    tb.setCustomValidity("");
    tb.required = false;
  }
}
const form = document.querySelector("form") as HTMLFormElement;
const bfield = <HTMLInputElement>document.getElementById('brand-field');
const cfield = <HTMLInputElement>document.getElementById('category-field');
const tfield = <HTMLInputElement>document.getElementById('topic-field');
function wireClearOnInput(input: HTMLInputElement) {
  input.addEventListener("input", () => {
    input.setCustomValidity("");
  });
}

if (bfield) wireClearOnInput(bfield);
if (cfield) wireClearOnInput(cfield);
if (tfield) wireClearOnInput(tfield);


form.addEventListener("submit", (e: Event) => {
  const fields = [
    { el: bfield, msg: "Please enter a valid brand." },
    { el: cfield, msg: "Please enter a valid category." },
    { el: tfield, msg: "Please enter a valid topic." }
  ];

  for (const f of fields) {
    if (f.el.required) {
      if (!f.el.value.trim()) {
        e.preventDefault();
        f.el.setCustomValidity(f.msg);
        f.el.reportValidity();
        f.el.focus();
        return;
      } else {
        f.el.setCustomValidity("");
      }
    }
  }
});
