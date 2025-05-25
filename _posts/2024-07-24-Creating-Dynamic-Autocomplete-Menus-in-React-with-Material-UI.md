---
layout: post
title: Creating Dynamic Autocomplete Menus in React with Material-UI
date: 2024-07-24

tags: react autocomplete javascript webdevelopment frontend material-ui userexperience uicomponents webdesign softwaredevelopment tech gpt-4o
categories: programming
---

# Enhancing User Experience with React: Building an Input with Autocomplete Suggestions

Autocomplete functionality can significantly improve user experience by providing suggestions as users type. In this article, we will explore how to create an input field with an autocomplete menu using React and Material-UI.

## Main Points of Functionality

### 1. Setting Up State and References

We manage the state using React's `useState` hook to keep track of input values, the status of the suggestion menu, suggested items, and the cursor position. We also use `useRef` to reference the input field.

```javascript
const [inputValue, setInputValue] = useState('');
const [suggestionMenuOpen, setSuggestionMenuOpen] = useState(false);
const [suggestedItems, setSuggestedItems] = useState<any[]>([]);
const [cursorPosition, setCursorPosition] = useState({ top: 0, left: 0 });
const textInputRef = useRef<HTMLInputElement | null>(null);
```

### 2. Suggestion Options

We define the options for the autocomplete suggestions. These options will be filtered based on user input.

```javascript
const suggestionOptions = [
  { value: 'GoogleSearch', text: 'Google Search' },
  { value: 'DatabaseLookup', text: 'Database Lookup' },
];
```

### 3. Calculating Caret Coordinates

The caret position is calculated to position the suggestion menu accurately. This involves creating a temporary div and span to measure the width of the text preceding the caret.

```javascript
const calculateCaretCoordinates = (element: HTMLInputElement, position: number) => {
  // Implementation to calculate and return caret coordinates
};
```

### 4. Handling Input Changes

The handleInputChange function is responsible for updating the input value, detecting the "@" symbol, filtering suggestion options, and updating the cursor position to show the suggestion menu.

```javascript
const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  const newValue = e.target.value;
  setInputValue(newValue);
  // Further implementation to detect "@" and filter suggestions
};
```

### 5. Handling Key Presses

The handleKeyPress function manages keyboard interactions, such as closing the suggestion menu on Escape, selecting a suggestion on Enter, and preventing default behavior for certain keys.

```javascript
const handleKeyPress = (e: KeyboardEvent<HTMLInputElement | HTMLTextAreaElement>) => {
  if (e.key === 'Escape') return setSuggestionMenuOpen(false);
  if (suggestionMenuOpen && e.key === 'Enter') {
    e.preventDefault();
    if (suggestedItems.length > 0) handleSuggestionSelect(suggestedItems[0].value);
  } else if (e.key === 'Enter' && !e.shiftKey) e.preventDefault();
};
```

### 6. Selecting a Suggestion

When a suggestion is selected, the menu closes, and the selected tool is logged.

```javascript
const handleSuggestionSelect = (item: string) => {
  setSuggestionMenuOpen(false);
  console.log(`Selected tool: ${item}`);
};
```

### 7. Rendering Components

The InputBase component from Material-UI is used for the input field, and the Popover component is used to display the suggestion menu. The menu items are mapped from the filtered suggestions.

```javascript
return (
  <>
    <InputBase
      multiline
      autoFocus
      inputRef={textInputRef}
      maxRows={10}
      placeholder="Type something..."
      value={inputValue}
      onChange={handleInputChange}
      onKeyDown={handleKeyPress}
    />
    <Popover
      anchorReference="anchorPosition"
      anchorPosition={{ top: cursorPosition.top, left: cursorPosition.left }}
      open={suggestionMenuOpen}
      onClose={() => setSuggestionMenuOpen(false)}
      anchorOrigin={{ vertical: 'top', horizontal: 'left' }}
      transformOrigin={{ vertical: 'bottom', horizontal: 'left' }}
      disableAutoFocus
      disableEnforceFocus
      disableRestoreFocus>
      {suggestedItems.map((item, index) => (
        <MenuItem key={item.value} onClick={() => handleSuggestionSelect(item.value)}>
          {item.text}
        </MenuItem>
      ))}
    </Popover>
  </>
);
```

## Conclusion

Implementing autocomplete functionality in React can enhance user experience by providing relevant suggestions as users type. This article walked through the main points of building an input field with an autocomplete menu using React and Material-UI.

For the complete code, you can refer to the following links:
- [View the code on PlayCode](https://playcode.io/1948870)
- [Download the code as a ZIP file](./assets/InputWithSuggestionMenu.zip)

> Created by gpt-4o v2024-05-13, reviewed and edited by me
