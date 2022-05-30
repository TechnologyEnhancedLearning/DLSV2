/**
 * @jest-environment jsdom
 */

import '../../learningMenu/diagnosticContentViewer';

beforeAll(() => {
  global.window = Object.create(window);
  const url = 'https://example.com';
  Object.defineProperty(global.window, 'location', {
    value: {
      href: url,
    },
  });
});

describe('closeMpe', () => {
  it(
    'should redirect to diagnostic assessment with no checked tutorials',
    () => {
      // Given
      window.location.href = 'https://localhost:44363/test/LearningMenu/123/456/Diagnostic/Content';

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toBe('https://localhost:44363/test/LearningMenu/123/456/Diagnostic');
    },
  );

  it(
    'should redirect to diagnostic assessment with no checked tutorials after entering fullscreen',
    () => {
      // Given
      window.location.href = 'https://localhost:44363/test/LearningMenu/123/456/Diagnostic/Content#';

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toBe('https://localhost:44363/test/LearningMenu/123/456/Diagnostic');
    },
  );

  it(
    'should redirect to diagnostic assessment with one checked tutorial',
    () => {
      // Given
      window.location.href = 'https://localhost:44363/test/LearningMenu/123/456/Diagnostic/Content?checkedTutorials=1234';

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toBe('https://localhost:44363/test/LearningMenu/123/456/Diagnostic');
    },
  );

  it(
    'should redirect to diagnostic assessment with one checked tutorial after entering fullscreen',
    () => {
      // Given
      window.location.href = 'https://localhost:44363/test/LearningMenu/123/456/Diagnostic/Content?checkedTutorials=1234#';

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toBe('https://localhost:44363/test/LearningMenu/123/456/Diagnostic');
    },
  );

  it(
    'should redirect to diagnostic assessment with multiple checked tutorials',
    () => {
      // Given
      window.location.href = 'https://localhost:44363/test/LearningMenu/123/456/Diagnostic/Content?checkedTutorials=123&checkedTutorials=456';

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toBe('https://localhost:44363/test/LearningMenu/123/456/Diagnostic');
    },
  );

  it(
    'should redirect to diagnostic assessment with multiple checked tutorials after entering fullscreen',
    () => {
      // Given
      window.location.href = 'https://localhost:44363/test/LearningMenu/123/456/Diagnostic/Content?checkedTutorials=123&checkedTutorials=456#';

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toBe('https://localhost:44363/test/LearningMenu/123/456/Diagnostic');
    },
  );

  it(
    'should do nothing on unexpected page',
    () => {
      // Given
      const url = 'https://localhost:44363/LearningMenu/123/456';
      window.location.href = url;

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toEqual(url);
    },
  );
});
