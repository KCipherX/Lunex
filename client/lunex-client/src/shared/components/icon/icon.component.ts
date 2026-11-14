import { Component, computed, input } from '@angular/core';

export type IconName =
  | 'users'
  | 'chat'
  | 'shield-check'
  | 'user-plus'
  | 'info'
  | 'plus'
  | 'edit'
  | 'chevron-left'
  | 'chevron-right'
  | 'send'
  | 'close'
  | 'check'
  | 'heart';

type IconSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-icon',
  standalone: true,
  template: `
    <svg
      [class]="sizeClasses()"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      stroke-width="1.8"
      stroke-linecap="round"
      stroke-linejoin="round"
      aria-hidden="true"
      focusable="false"
    >
      @switch (name()) {
        @case ('users') {
          <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
          <circle cx="9" cy="7" r="4" />
          <path d="M22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75" />
        }
        @case ('chat') {
          <path d="M21 11.5a8.4 8.4 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.4 8.4 0 0 1-3.8-.9L3 21l1.9-5.7a8.4 8.4 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.4 8.4 0 0 1 3.8-.9h.5a8.5 8.5 0 0 1 8 8z" />
        }
        @case ('shield-check') {
          <path d="M12 22s8-4 8-11V5l-8-3-8 3v6c0 7 8 11 8 11z" />
          <path d="m9 12 2 2 4-4" />
        }
        @case ('user-plus') {
          <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
          <circle cx="9" cy="7" r="4" />
          <path d="M19 8v6m3-3h-6" />
        }
        @case ('info') {
          <circle cx="12" cy="12" r="10" />
          <path d="M12 16v-4m0-4h.01" />
        }
        @case ('plus') {
          <path d="M12 5v14m7-7H5" />
        }
        @case ('edit') {
          <path d="M12 20h9" />
          <path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L8 18l-4 1 1-4Z" />
        }
        @case ('chevron-left') {
          <path d="m15 18-6-6 6-6" />
        }
        @case ('chevron-right') {
          <path d="m9 18 6-6-6-6" />
        }
        @case ('send') {
          <path d="m22 2-7 20-4-9-9-4Z" />
          <path d="M22 2 11 13" />
        }
        @case ('close') {
          <path d="m18 6-12 12M6 6l12 12" />
        }
        @case ('check') {
          <path d="m5 12 4 4L19 6" />
        }
        @case ('heart') {
          <path d="M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.7l-1.1-1.1a5.5 5.5 0 0 0-7.8 7.8l1.1 1.1L12 21l7.8-7.5 1.1-1.1a5.5 5.5 0 0 0-.1-7.8z" />
        }
      }
    </svg>
  `,
})
export class IconComponent {
  name = input<IconName | ''>('');
  size = input<IconSize>('md');

  sizeClasses = computed(() => {
    const sizes: Record<IconSize, string> = {
      sm: 'h-4 w-4',
      md: 'h-5 w-5',
      lg: 'h-6 w-6',
    };

    return sizes[this.size()];
  });
}
