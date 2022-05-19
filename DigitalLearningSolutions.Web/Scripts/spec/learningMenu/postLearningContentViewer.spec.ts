/**
 * @jest-environment jsdom
 */

import '../../learningMenu/postLearningContentViewer';

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
    'should redirect to post learning assessment',
    () => {
      // Given
      window.location.href = 'https://localhost:44363/test/LearningMenu/123/456/PostLearning/Content';

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toBe('https://localhost:44363/test/LearningMenu/123/456/PostLearning');
    },
  );

  it(
    'should redirect to post learning assessment after entering fullscreen',
    () => {
      // Given
      window.location.href = 'https://localhost:44363/test/LearningMenu/123/456/PostLearning/Content#';

      // When
      window.closeMpe();

      // Then
      expect(window.location.href).toBe('https://localhost:44363/test/LearningMenu/123/456/PostLearning');
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
