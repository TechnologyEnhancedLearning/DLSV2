import * as jodit from 'jodit';

let jodited = false;
if (jodited === false) {
  const editor = new jodit.Jodit('.html-editor', {
    buttons: [
      'source', '|',
      'bold',
      'italic', '|',
      'ul',
      'ol', '|',
      'table',
      'link', '|',
      'left',
      'center',
      'right',
      'justify', '|',
      'undo', 'redo', '|',
      'hr',
    ],
    buttonsMD: [
      'source', '|',
      'bold',
      'italic', '|',
      'ul',
      'ol', '|',
      'table',
      'link', '|',
      'left',
      'center',
      'right',
      'justify', '|',
      'undo', 'redo', '|',
      'hr',
    ],
    buttonsSM: [
      'bold',
      'italic', '|',
      'ul',
      'ol', '|',
      'table',
      'link', '|',
      'left',
      'center',
      'right',
      'justify', '|',
      'undo', 'redo', '|',
      'hr',
    ],
    buttonsXS: [
      'bold',
      'italic', '|',
      'ul',
      'ol', '|',
      'undo', 'redo',
    ],
  });
  if (editor != null) {
    jodited = true;
  }
}
