---
layout: post
title: Testing React Components with Vitest - Testing React Components with Vitest
date: 2025-01-19

tags: testing react vitest components mocking hooks
categories: programming testing
---

# Testing React Components with Vitest

## Difference Between vi.mock and vi.fn

| **Aspect** | **`vi.spyOn`** | **`vi.fn` (mock)** |
| ---- | ---- | ---- |
| **Behavior** | Observes an existing function. | Replaces a function entirely. |
| **Preserves logic** | Yes, by default. | No, unless you explicitly define it. |
| **Flexibility** | Modifies existing methods conditionally. | Fully customizable, starting from scratch. |
| **Typical use case** | Tracking calls to real methods. | Testing isolated components or behaviors. |

## Testing Interactive Components

### Counter Button component

```tsx
import { useState } from "react";

export default function CounterButton() {
  const [count, setCount] = useState(0);
  return (
    <div>
      <span>{count}</span>
      <button onClick={() => setCount(count + 1)}>Increment</button>
    </div>
  );
}
```

### Counter Button test

```tsx
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import CounterButton from "./button-click.component.tsx";
import { expect, test } from "vitest";
import "@testing-library/jest-dom";

test("increments value when button is clicked", async () => {
  render(<CounterButton />);

  const user = userEvent.setup();
  const button = screen.getByRole("button", { name: "Increment" });
  const value = screen.getByText("0");

  await user.click(button);

  expect(value).toHaveTextContent("1");
});
```

## Mocking Global APIs (e.g., fetch)

### Fetch Button component

```tsx
import { useState } from "react";

export default function FetchButton() {
  const [data, setData] = useState(null);

  async function handleClick() {
    const response = await fetch("/api/data");
    const result = await response.json();
    setData(result.message);
  }

  return (
    <div>
      <button onClick={handleClick}>Fetch Data</button>
      {data && <span>{data}</span>}
    </div>
  );
}

```

### Fetch Button test

```tsx
import { expect, test, vi } from "vitest";
import FetchButton from "./mock-fetch.component";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import "@testing-library/jest-dom";

test("calls fetch when button is clicked", async () => {
  // Mock the fetch function
  vi.stubGlobal(
    "fetch",
    vi.fn(() =>
      Promise.resolve({
        json: () => Promise.resolve({ message: "Hello, World!" }),
      })
    )
  );
  render(<FetchButton />);
  const user = userEvent.setup();

  const button = screen.getByRole("button", { name: "Fetch Data" });

  // Click the button
  await user.click(button);

  // Verify fetch was called with the correct URL
  expect(fetch).toHaveBeenCalledWith("/api/data");

  // Verify the rendered data
  const result = await screen.findByText("Hello, World!");
  expect(result).toBeInTheDocument();

  // Cleanup mock
  vi.unstubAllGlobals();
});

```

## Testing Callbacks Passed to Child Components

### Parent & Child components

```tsx
import ChildComponent from "./child.component.tsx";

const ParentComponent = () => {
  const onCallback1 = () => {
    console.log("Callback 1 triggered");
  };

  const onCallback2 = () => {
    console.log("Callback 2 triggered");
  };

  return (
    <div>
      <h1>Parent Component</h1>
      <ChildComponent onCallback1={onCallback1} onCallback2={onCallback2} showTitle={true} />
    </div>
  );
};

export default ParentComponent;

```

```tsx
export interface ChildComponentProps {
  onCallback1: () => void;
  onCallback2: () => void;
  showTitle: boolean;
}

const ChildComponent = ({ showTitle, onCallback1, onCallback2 }: ChildComponentProps) => {
  return (
    <div>
      {showTitle && <h1>Child Component</h1>}
      <button onClick={onCallback1}>Callback 1</button>
      <button onClick={onCallback2}>Callback 2</button>
    </div>
  );
};

export default ChildComponent;

```

### Parent test

```tsx
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { expect, test, vi } from "vitest";
import ParentComponent from "./parent.component.tsx";
import { ChildComponentProps } from "./child.component.tsx";
import ChildComponent from "./child.component.tsx";

// Mock the child component
vi.mock("./child.component.tsx", async () => {
  return {
    default: vi.fn().mockImplementation(({ onCallback1, onCallback2 }: ChildComponentProps) => (
      <div>
        <button data-testid="button-1" onClick={onCallback1}>
          Trigger Callback 1
        </button>
        <button data-testid="button-2" onClick={onCallback2}>
          Trigger Callback 2
        </button>
      </div>
    )),
  };
});

test("calls parent's internal callbacks when child buttons are clicked", async () => {
  const user = userEvent.setup();

  // Spy on console.log to check if callbacks are triggered
  const consoleSpy = vi.spyOn(console, "log");

  render(<ParentComponent />);

  // Simulate button clicks in the mocked child component
  const button1 = screen.getByTestId("button-1");
  const button2 = screen.getByTestId("button-2");

  await user.click(button1);
  await user.click(button2);

  // Assert the parent's callbacks (console logs) are triggered
  expect(consoleSpy).toHaveBeenCalledWith("Callback 1 triggered");
  expect(consoleSpy).toHaveBeenCalledWith("Callback 2 triggered");
  expect(vi.mocked(ChildComponent)).toHaveBeenCalledWith(
    expect.objectContaining({
      showTitle: true, // Verifying the showTitle prop
    }),
    expect.anything()
  );

  // Cleanup the console spy
  consoleSpy.mockRestore();
});

```

## Mocking Custom Hooks

### Parent component & Custom hook

```tsx
import useCustomHook from "./useCustomHook";

const ParentComponent = () => {
  const { data, isLoading } = useCustomHook();

  if (isLoading) {
    return <p>Loading...</p>;
  }

  return <div>{data ? <p>Data: {data}</p> : <p>No Data Available</p>}</div>;
};

export default ParentComponent;


```

```tsx
import { useState, useEffect } from "react";

export interface CustomHookReturn {
  data: string | null;
  isLoading: boolean;
}

export default function useCustomHook(): CustomHookReturn {
  const [data, setData] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    setTimeout(() => {
      setData("Hello, World!");
      setIsLoading(false);
    }, 1000);
  }, []);

  return { data, isLoading };
}

```

### Parent test

```tsx
import { render, screen } from "@testing-library/react";
import { test, vi, expect } from "vitest";
import ParentComponent from "./parent.component";
import useCustomHook from "./useCustomHook";
import "@testing-library/jest-dom";

// Mock the custom hook
vi.mock("./useCustomHook", async () => {
  return {
    default: vi.fn(),
  };
});

test("shows loading state initially", () => {
  // Mock the custom hook's return value for loading state
  vi.mocked(useCustomHook).mockReturnValue({
    data: null,
    isLoading: true,
  });

  render(<ParentComponent />);

  // Assert the loading state is rendered
  expect(screen.getByText("Loading...")).toBeInTheDocument();
});

test("shows data when available", () => {
  // Mock the custom hook's return value for data state
  vi.mocked(useCustomHook).mockReturnValue({
    data: "Hello, World!",
    isLoading: false,
  });

  render(<ParentComponent />);

  // Assert the data is rendered
  expect(screen.getByText("Data: Hello, World!")).toBeInTheDocument();
});

test("shows no data message when data is null", () => {
  // Mock the custom hook's return value for no data state
  vi.mocked(useCustomHook).mockReturnValue({
    data: null,
    isLoading: false,
  });

  render(<ParentComponent />);

  // Assert the "No Data Available" message is rendered
  expect(screen.getByText("No Data Available")).toBeInTheDocument();
});

```

## Sources

- [Samples](https://github.com/ypyl/react-vitest-samples/tree/master/vite-project/src/components)
- [Mocking | Guide | Vitest](https://vitest.dev/guide/mocking)
- [Example | Testing Library](https://testing-library.com/docs/react-testing-library/example-intro)
