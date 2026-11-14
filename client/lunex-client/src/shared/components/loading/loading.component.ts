import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

type SpinnerSize = 'sm' | 'md' | 'lg' | 'xl';
type SpinnerType = 'spinner' | 'dots' | 'ring' | 'ball';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (type() === 'spinner') {
      <svg
        [class]="spinnerClasses()"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
      >
        <circle
          class="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          stroke-width="4"
        ></circle>
        <path
          class="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
        ></path>
      </svg>
    } @else if (type() === 'dots') {
      <span [class]="dotsClasses()" class="loading loading-dots"></span>
    } @else if (type() === 'ring') {
      <span [class]="ringClasses()" class="loading loading-ring"></span>
    } @else if (type() === 'ball') {
      <span [class]="ballClasses()" class="loading loading-ball"></span>
    }
  `,
})
export class LoadingComponent {
  size = input<SpinnerSize>('md');
  type = input<SpinnerType>('spinner');
  color = input<'primary' | 'secondary' | 'accent' | 'info' | 'success' | 'warning' | 'error'>('primary');
  inline = input(false);

  spinnerClasses = computed(() => {
    const sizes = {
      sm: 'h-4 w-4',
      md: 'h-8 w-8',
      lg: 'h-12 w-12',
      xl: 'h-16 w-16',
    };
    const colorClass = `text-${this.color()}`;
    const inlineClass = this.inline() ? 'inline-block' : 'block';
    const animate = 'animate-spin';
    
    return `${sizes[this.size()]} ${colorClass} ${inlineClass} ${animate}`;
  });

  dotsClasses = computed(() => {
    const sizes = {
      sm: 'loading-xs',
      md: 'loading-sm',
      lg: 'loading-lg',
      xl: 'loading-xl',
    };
    const colorClass = `text-${this.color()}`;
    return `${sizes[this.size()]} ${colorClass}`;
  });

  ringClasses = computed(() => {
    const sizes = {
      sm: 'loading-xs',
      md: 'loading-sm',
      lg: 'loading-lg',
      xl: 'loading-xl',
    };
    const colorClass = `text-${this.color()}`;
    return `${sizes[this.size()]} ${colorClass}`;
  });

  ballClasses = computed(() => {
    const sizes = {
      sm: 'loading-xs',
      md: 'loading-sm',
      lg: 'loading-lg',
      xl: 'loading-xl',
    };
    const colorClass = `text-${this.color()}`;
    return `${sizes[this.size()]} ${colorClass}`;
  });
}