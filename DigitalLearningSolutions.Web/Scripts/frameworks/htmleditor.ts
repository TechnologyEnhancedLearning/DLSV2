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

    document.addEventListener('DOMContentLoaded', () => {
      removeWaveErrors();
      removeDevToolsIssues();
    });

    // ** Start* for jodit editor error (display red outline, focus on summary error text click) ****
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
    // ** End* for jodit editor error (display red outline, focus on summary error text click) ****
  }
}

function removeWaveErrors() {
  const input = Array.from(document.querySelectorAll<HTMLInputElement>('input[tab-index="-1"]'))
    .find((el) => el.style.width === '0px' && el.style.height === '0px'
      && el.style.position === 'absolute' && el.style.visibility === 'hidden');

  if (input) {
    input.setAttribute('aria-label', 'Hidden input for accessibility');
    input.setAttribute('title', 'HiddenInput');
  }

  const observer = new MutationObserver((mutations, obs) => {
    const textarea = document.querySelector('.ace_text-input') as HTMLTextAreaElement | null;
    if (textarea) {
      textarea.setAttribute('aria-label', 'ace_text-input');
      obs.disconnect();
    }
  });
  observer.observe(document.body, {
    childList: true,
    subtree: true,
  });
}
function removeDevToolsIssues() {
  // set role = 'list' to toolbar
  const toolbarbox = document.querySelector('.jodit-toolbar__box') as HTMLElement | null;
  if (toolbarbox) {
    toolbarbox.setAttribute('role', 'list');
  }
  // set role = 'list' to statusbar
  const statusbar = document.querySelector('.jodit-xpath') as HTMLElement | null;
  if (statusbar) {
    statusbar.setAttribute('role', 'list');
  }
  document.querySelectorAll('.jodit-toolbar-button__trigger').forEach((el) => {
    el.removeAttribute('role');
  });
  // observer to detect role='trigger' and remove role
  const observer = new MutationObserver(() => {
    document.querySelectorAll('.jodit-toolbar-button__trigger').forEach((el) => {
      el.removeAttribute('role');
    });
  });
  const target = document.querySelector('.jodit-toolbar__box');
  if (target) {
    observer.observe(target, { subtree: true, childList: true });
  }

  // observer to detect iframe and set title
  const observer2 = new MutationObserver(() => {
    const hiddenIframe = Array.from(document.querySelectorAll('iframe')).find((iframe) => {
      const rect = iframe.getBoundingClientRect();
      return rect.width === 0 && rect.height === 0 && (iframe.src === 'about:blank' || iframe.getAttribute('src') === 'about:blank');
    });
    if (hiddenIframe) {
      hiddenIframe.setAttribute('title', 'Hidden iframe');
      observer2.disconnect(); // Stop observing once found
    }
  });
  observer2.observe(document.body, {
    childList: true,
    subtree: true,
  });
}
