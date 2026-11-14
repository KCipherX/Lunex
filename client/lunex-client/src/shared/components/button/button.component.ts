import { Component, input, output, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent, IconName } from '../icon/icon.component';
import { RouterLink } from '@angular/router';

type ButtonVariant = 'primary' | 'secondary' | 'outline' | 'ghost' | 'error' | 'success';
type ButtonSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule, IconComponent, RouterLink],
  template: `
    <button
      [type]="type()"
      [routerLink]="routerLink()"
      [disabled]="disabled() || loading()"
      [attr.aria-busy]="loading()"
      [class]="buttonClasses()"
      (click)="onClick.emit($event)"
    >
      @if (loading()) {
        <svg class="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24" aria-hidden="true">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
      } @else if (icon() && iconPosition() === 'left') {
        <app-icon [name]="icon()" size="sm" />
      }
      <span>{{ label() }}</span>
      @if (!loading() && icon() && iconPosition() === 'right') {
        <app-icon [name]="icon()" size="sm" />
      }
    </button>
  `,
  host: {
    '[class.button-full-width]': 'fullWidth()',
  },
})
export class ButtonComponent {
  label = input.required<string>();
  variant = input<ButtonVariant>('primary');
  size = input<ButtonSize>('md');
  type = input<'button' | 'submit' | 'reset'>('button');
  disabled = input(false);
  loading = input(false);
  icon = input<IconName | ''>('');
  routerLink = input<string | null>(null);
  fullWidth = input(false);
  iconPosition = input<'left' | 'right'>('left');
  buttonClass = input('');

  onClick = output<MouseEvent>();

  buttonClasses = computed(() => {
    const base = 'inline-flex items-center justify-center gap-2 font-semibold transition-colors duration-200 focus:outline-none disabled:cursor-not-allowed';
    
    const variants: Record<ButtonVariant, string> = {
      primary: 'btn btn-primary',
      secondary: 'btn btn-secondary',
      outline: 'btn btn-outline',
      ghost: 'btn btn-ghost',
      error: 'btn btn-error',
      success: 'btn btn-success',
    };

    const sizes: Record<ButtonSize, string> = {
      sm: 'btn-sm',
      md: '',
      lg: 'btn-lg',
    };

    const width = this.fullWidth() ? 'w-full' : '';

    return `${base} ${variants[this.variant()]} ${sizes[this.size()]} ${width} ${this.buttonClass()}`;
  });
}