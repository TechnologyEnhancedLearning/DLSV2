﻿import * as jodit from "jodit";
const editor = new jodit.Jodit('#competency-description', {
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
    'hr'
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
    'hr'
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
    'hr'
  ],
  buttonsXS: [
    'bold',
    'italic', '|',
    'ul',
    'ol', '|',
    'undo', 'redo'
  ]});
