import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

type AvatarSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl';
type AvatarShape = 'circle' | 'square';

@Component({
  selector: 'app-avatar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      [class]="avatarClasses()"
      [style.background-image]="backgroundImage()"
      [attr.role]="alt() || text() ? 'img' : null"
      [attr.aria-label]="alt() || text() || null"
    >
      @if (!src() && !text()) {
        <svg xmlns="http://www.w3.org/2000/svg" class="h-full w-full text-base-content/30" fill="none" viewBox="0 0 24 24" stroke="currentColor" aria-hidden="true">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5.121 17.804A13.937 13.937 0 0112 16c2.5 0 4.847.655 6.879 1.804M15 10a3 3 0 11-6 0 3 3 0 016 0zm6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
      }
      @if (text()) {
        <span class="font-medium text-base-content select-none">{{ text() }}</span>
      }
      @if (status() && status() !== '') {
        <span [class]="statusClasses()" aria-hidden="true"></span>
      }
    </div>
  `,
})
export class AvatarComponent {
  src = input<string>('');
  alt = input<string>('');
  text = input<string>('');
  size = input<AvatarSize>('md');
  shape = input<AvatarShape>('circle');
  status = input<'online' | 'offline' | 'busy' | 'away' | ''>('');
  statusPosition = input<'bottom-right' | 'bottom-left' | 'top-right' | 'top-left'>('bottom-right');

  backgroundImage = computed(() => {
    return this.src() ? `url(${this.src()})` : 'none';
  });

  avatarClasses = computed(() => {
    const sizes: Record<AvatarSize, string> = {
      xs: 'h-6 w-6 text-xs',
      sm: 'h-8 w-8 text-sm',
      md: 'h-10 w-10 text-base',
      lg: 'h-12 w-12 text-lg',
      xl: 'h-16 w-16 text-xl',
      '2xl': 'h-20 w-20 text-2xl',
    };

    const shapes: Record<AvatarShape, string> = {
      circle: 'rounded-full',
      square: 'rounded-lg',
    };

    const base = 'inline-flex items-center justify-center overflow-hidden bg-base-200 text-base-content relative shrink-0';
    
    return `${base} ${sizes[this.size()]} ${shapes[this.shape()]}`;
  });

  statusClasses = computed(() => {
    const status = this.status();
    if (!status) return 'hidden';
    
    const positions = {
      'bottom-right': 'absolute bottom-0 right-0',
      'bottom-left': 'absolute bottom-0 left-0',
      'top-right': 'absolute top-0 right-0',
      'top-left': 'absolute top-0 left-0',
    };

    const colors = {
      online: 'bg-success',
      offline: 'bg-base-content/30',
      busy: 'bg-error',
      away: 'bg-warning',
    };

    const size = this.size();
    const statusSize = {
      xs: 'h-2 w-2',
      sm: 'h-2.5 w-2.5',
      md: 'h-3 w-3',
      lg: 'h-3.5 w-3.5',
      xl: 'h-4 w-4',
      '2xl': 'h-5 w-5',
    };

    return `${positions[this.statusPosition()]} ${colors[status]} ${statusSize[size]} rounded-full border-2 border-base-100`;
  });
}