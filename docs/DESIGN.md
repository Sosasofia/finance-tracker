---
name: FinanceTracker
description: A warm, softly rounded finance workspace with clear semantic signals.
colors:
  primary: "#4f46e5"
  primary-light: "rgba(79, 70, 229, 0.1)"
  income: "#2e7d32"
  income-light: "rgba(16, 185, 129, 0.1)"
  expense: "#c62828"
  expense-light: "rgba(239, 68, 68, 0.1)"
  text-main: "#111827"
  text-muted: "#6b7280"
  page-bg: "#f9f9f9"
  surface: "#fafafa"
  surface-white: "#ffffff"
  line: "#e0e0e0"
  soft-line: "#f0f0f0"
  control-bg: "#f9fafb"
  icon-bg: "#f5f6f8"
typography:
  body:
    fontFamily: "Roboto, Helvetica Neue, sans-serif"
    fontSize: "16px"
    fontWeight: 400
    lineHeight: 1.5
  headline:
    fontFamily: "Roboto, Helvetica Neue, sans-serif"
    fontSize: "24px"
    fontWeight: 700
    lineHeight: 1.2
  balance:
    fontFamily: "Roboto, Helvetica Neue, sans-serif"
    fontSize: "38px"
    fontWeight: 700
    lineHeight: 1.2
    letterSpacing: "-0.5px"
  title:
    fontFamily: "Roboto, Helvetica Neue, sans-serif"
    fontSize: "16px"
    fontWeight: 700
    lineHeight: 1.25
  label:
    fontFamily: "Roboto, Helvetica Neue, sans-serif"
    fontSize: "12px"
    fontWeight: 500
    lineHeight: 1.3
rounded:
  sm: "8px"
  md: "12px"
  lg: "20px"
  xl: "24px"
  dialog: "28px"
  pill: "50%"
spacing:
  page-mobile: "12px"
  page-tablet: "16px"
  page-desktop: "24px"
  control: "12px"
  card: "24px"
  section: "24px"
components:
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.surface-white}"
    rounded: "{rounded.xl}"
    padding: "12px 24px"
  button-income:
    backgroundColor: "{colors.income-light}"
    textColor: "{colors.income}"
    rounded: "{rounded.md}"
    padding: "12px 16px"
  button-expense:
    backgroundColor: "{colors.expense-light}"
    textColor: "{colors.expense}"
    rounded: "{rounded.md}"
    padding: "12px 16px"
  card:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.text-main}"
    rounded: "{rounded.xl}"
    padding: "{spacing.card}"
  input:
    backgroundColor: "{colors.surface-white}"
    textColor: "{colors.text-main}"
    rounded: "{rounded.md}"
    padding: "0 12px"
  nav-active:
    backgroundColor: "{colors.primary-light}"
    textColor: "{colors.primary}"
    rounded: "{rounded.lg}"
    padding: "10px 14px"
---

# Design System: FinanceTracker

## Overview

**Creative North Star: "Soft Signal Dashboard"**

FinanceTracker is a warm, approachable workspace for maintaining financial clarity. Its visual language uses gentle neutral surfaces, softly rounded containers, and a restrained indigo accent so everyday money work feels reassuring rather than intimidating. Roboto keeps the interface familiar and legible while semantic green and red signals make income and expense states immediately scannable.

The system favors calm control over spectacle: information is organized into generous cards and simple rows, with ambient depth instead of heavy decoration. It should feel friendly and low-friction, never like a dense spreadsheet or a harsh enterprise console.

**Key Characteristics:**
- Light, warm, and approachable
- Softly rounded surfaces and controls
- Clear semantic income and expense signals
- Ambient layering with restrained shadows
- Responsive navigation that becomes a mobile bottom bar

## Colors

The palette is neutral-first with one quiet indigo accent and explicit green/red semantic states.

### Primary
- **Quiet Ledger Indigo** (`#4f46e5`): The main action and navigation accent, used for active navigation, primary controls, focus states, and chart emphasis.
- **Quiet Ledger Indigo Wash** (`rgba(79, 70, 229, 0.1)`): A low-intensity indigo field for active navigation, focus halos, and selected states.

### Secondary
- **Income Green** (`#2e7d32`): Positive financial movement, income totals, and income actions.
- **Income Green Wash** (`rgba(16, 185, 129, 0.1)`): The soft background for income actions and positive summary signals.

### Tertiary
- **Expense Red** (`#c62828`): Negative financial movement, expense totals, destructive/logout emphasis, and expense actions.
- **Expense Red Wash** (`rgba(239, 68, 68, 0.1)`): The soft background for expense actions and negative summary signals.

### Neutral
- **Ink** (`#111827`): Primary text, balances, and high-importance labels.
- **Muted Slate** (`#6b7280`): Supporting text, metadata, and secondary icon states.
- **Warm Page Mist** (`#f9f9f9`): Global page background.
- **Soft Surface** (`#fafafa`): Default card surface.
- **White Surface** (`#ffffff`): Controls, mobile balance cards, dialogs, and elevated interaction surfaces.
- **Quiet Line** (`#e0e0e0`): Card borders and structural separators.
- **Hairline** (`#f0f0f0`): Transaction row dividers.
- **Control Mist** (`#f9fafb`): Search fields and lightweight controls.
- **Icon Mist** (`#f5f6f8`): Circular transaction icon backgrounds.

### Named Rules
**The Signal Rarity Rule.** Use indigo, green, and red to communicate action or financial meaning; keep the rest of the canvas quiet.

## Typography

**Display Font:** Roboto (with Helvetica Neue, sans-serif fallback)
**Body Font:** Roboto (with Helvetica Neue, sans-serif fallback)
**Label/Mono Font:** No distinct label or mono face; labels remain in Roboto.

**Character:** Familiar, clean, and practical. Weight and size create hierarchy more than decorative type treatment.

### Hierarchy
- **Balance** (700, 38px, 1.2 line-height, -0.5px tracking): The dominant financial figure on the dashboard.
- **Headline** (700, 24px, 1.2 line-height): Page titles such as Dashboard and Analytics.
- **Title** (700, 16px, 1.25 line-height): Card titles and important transaction labels.
- **Body** (400, 16px, 1.5 line-height): General interface copy and form content.
- **Label** (500, 12px, 1.3 line-height): Metadata, subtitles, and compact supporting information.

### Named Rules
**The Familiar Numbers Rule.** Financial figures should be bold and easy to scan, while supporting metadata stays smaller and muted.

## Layout

The desktop shell uses a persistent Material sidenav with a constrained content area and a 24px page rhythm. Dashboard cards use a two-column grid with a 24px gap; the activity list sits below as a full-width card. Page and card padding is 24px on desktop, 16px at tablet widths, and 12px on mobile.

At widths below 900px, dashboard cards stack into one column and card padding tightens to 20px. At mobile widths, the sidenav becomes a compact header plus a fixed bottom navigation bar; page headings may be omitted to preserve vertical space. Preserve generous grouping and avoid turning the interface into a dense table.

## Elevation & Depth

This is a layered, ambient system. Pale tonal contrast and quiet borders do most of the structural work; shadows are deliberately soft and low-opacity. Depth should clarify containers and interaction states, not make the dashboard feel glossy or heavy.

### Shadow Vocabulary
- **Card ambient** (`0 4px 12px rgba(0, 0, 0, 0.02)`): Default separation for dashboard cards and transaction surfaces.
- **Control focus** (`0 0 0 3px rgba(79, 70, 229, 0.1)`): A soft indigo focus halo around searchable controls.
- **Floating menu** (`0 4px 12px rgba(0, 0, 0, 0.05)`): Slight lift for compact menus or floating interaction surfaces.

### Named Rules
**The Ambient Depth Rule.** Prefer a soft surface, border, or tonal shift before increasing shadow strength.

## Shapes

The form language is gently rounded and reassuring. Cards use 24px corners on desktop and 20px on narrower layouts; buttons and controls commonly use 12px or pill-like 24px corners. Dialogs use a generous 28px radius, while avatars and icon wells use circular silhouettes. Borders are quiet and functional rather than decorative.

## Components

### Buttons
- **Shape:** Softly rounded, from 12px for compact action buttons to 24px for Material dialog actions.
- **Primary:** Indigo action with white text, generous horizontal padding, and medium-to-bold weight.
- **Income / Expense:** Low-contrast semantic wash with matching green or red text; use for quick add actions and compact stateful actions.
- **Hover / Focus:** Subtle opacity or background change; preserve a visible indigo focus halo where the control is text-entry or navigation related.
- **Pressed:** Small scale response is used on quick action buttons (`scale(0.96)`); keep motion brief and restrained.

### Chips
- **Style:** Category filters use pill-like white controls with rounded 24px geometry and soft ambient separation.
- **State:** Selected categories use dark ink or active emphasis; unselected categories remain quiet and low-contrast.

### Cards / Containers
- **Corner Style:** 24px default, 20px at narrower widths, 28px for dialogs.
- **Background:** Soft Surface or White Surface depending on whether the container is a page card or an active interaction surface.
- **Shadow Strategy:** Use the Card ambient shadow and quiet borders from Elevation & Depth.
- **Border:** A subtle `#e0e0e0` border is acceptable for dashboard cards; mobile activity sections may become borderless and flat.
- **Internal Padding:** 24px desktop, 20px tablet/mobile cards, and 12px for compact activity sections.

### Inputs / Fields
- **Style:** Clean white or transparent-filled fields with 8–12px corners; avoid heavy chrome.
- **Focus:** A Quiet Ledger Indigo border or soft focus halo.
- **Error / Disabled:** Use explicit Material error treatment and preserve readable contrast; do not rely on color alone.

### Navigation
- **Desktop:** Vertical sidenav with a compact user header, 52px minimum-height menu items, and 20px rounded active rows.
- **Active:** Indigo wash background, indigo icon/text, and semibold weight.
- **Mobile:** Compact greeting header plus bottom navigation with icon and label; keep the active state clear without adding visual noise.

### Transaction Rows
- **Style:** Two-sided list rows with a circular Icon Mist well, bold transaction name, muted date metadata, and right-aligned semantic amount.
- **Behavior:** Separate rows with Hairline dividers; remove the final divider.

## Do's and Don'ts

### Do:
- **Do** keep the canvas light, calm, and neutral-first.
- **Do** reserve indigo for navigation, primary actions, focus, and selected emphasis.
- **Do** use green and red consistently for income and expense meaning.
- **Do** preserve soft 20–28px container geometry where the component is a major surface.
- **Do** keep mobile navigation explicit and easy to reach.
- **Do** use spacing and grouping to make financial information feel manageable.

### Don't:
- **Don't** turn the dashboard into a dense spreadsheet or compressed data grid.
- **Don't** use harsh black surfaces, neon accents, or glossy fintech gradients.
- **Don't** use sharp corners as the dominant silhouette.
- **Don't** use green or red as generic decoration without financial meaning.
- **Don't** increase shadow strength until tonal layering and borders have been considered.
- **Don't** replace familiar Roboto hierarchy with decorative display typography.
