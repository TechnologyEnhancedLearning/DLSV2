import { Jodit } from 'jodit';
import DOMPurify from 'dompurify';

let jodited = false;
if (jodited === false) {
  const editor = Jodit.make('.html-editor', {
    buttons: [
      'source', '|',
      'bold',
      'italic', '|',
      'ul',
      'ol',
      'outdent',
      'indent', '|',
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
      'ol',
      'outdent',
      'indent', '|',
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
    style: {
      backgroundColor: '#FFF',
    },
  });

  if (editor != null) {
    jodited = true;
    editor.e.on('blur', () => {
      const clean = DOMPurify.sanitize(editor.editor.innerHTML);
      editor.editor.innerHTML = clean;
    });
    const textarea = document.querySelector('.nhsuk-textarea.html-editor.nhsuk-input--error') as HTMLTextAreaElement | null;
    if (textarea) {
      const editorDiv = document.querySelector('.jodit-container.jodit.jodit_theme_default.jodit-wysiwyg_mode') as HTMLDivElement | null;
      editorDiv?.classList.add('jodit-container', 'jodit', 'jodit_theme_default', 'jodit-wysiwyg_mode', 'jodit-error');
    }

    const summary = document.querySelector('.nhsuk-list.nhsuk-error-summary__list') as HTMLDivElement | null;

    if (summary) {
      summary.addEventListener('click', (e: Event) => {
        if (textarea) {
          const textareaId = textarea.id.toString();
          const target = e.target as HTMLElement;
          if (target.tagName.toLowerCase() === 'a') {
            const href = (target as HTMLAnchorElement).getAttribute('href');

            if (href && href.includes(textareaId)) {
              const editorArea = document.querySelector('.jodit-wysiwyg') as HTMLDivElement | null;
              editorArea?.focus();
              editorArea?.scrollIntoView({ behavior: 'smooth', block: 'center' });
              e.preventDefault();
            }
          }
        }
      });
    }
  }
}
