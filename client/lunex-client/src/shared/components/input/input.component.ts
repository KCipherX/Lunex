import { Component, input, output, signal, computed, model, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IconComponent, IconName } from '../icon/icon.component';

type InputType = 'text' | 'email' | 'password' | 'number' | 'date' | 'tel' | 'url';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, IconComponent],
  template: `
    <div class="form-control w-full">
      @if (label()) {
        <label class="label" [attr.for]="fieldId()">
          <span class="label-text">{{ label() }}</span>
          @if (required()) {
            <span class="label-text-alt text-error">*</span>
          }
        </label>
      }
      
      <div class="relative">
        @if (icon()) {
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-base-content/50">
            <app-icon [name]="icon()" size="sm" />
          </div>
        }
        
        <input
          [type]="effectiveType()"
          [id]="fieldId()"
          [name]="name()"
          [placeholder]="placeholder()"
          [autocomplete]="autocomplete()"
          [disabled]="disabled()"
          [readonly]="readonly()"
          [required]="required()"
          [value]="value()"
          [class]="inputClasses()"
          [attr.aria-invalid]="error() ? 'true' : null"
          [attr.aria-describedby]="messageId()"
          (input)="onInput($event)"
          (blur)="onBlur()"
          (focus)="onFocus()"
          class="peer"
        />
        
        @if (showPasswordToggle() && (type() === 'password' || type() === 'text')) {
          <button
            type="button"
            class="absolute inset-y-0 right-0 pr-3 flex items-center text-base-content/50 hover:text-base-content"
            (click)="togglePassword()"
            (mousedown)="$event.preventDefault()"
          >
            @if (showPassword()) {
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
              </svg>
            } @else {
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
              </svg>
            }
          </button>
        }
      </div>
      
      @if (error()) {
        <p class="text-sm text-error mt-1" [id]="fieldId() + '-error'" role="alert">{{ error() }}</p>
      } @else if (hint()) {
        <p class="text-sm text-base-content/60 mt-1" [id]="fieldId() + '-hint'">{{ hint() }}</p>
      }
    </div>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true,
    },
  ],
})
export class InputComponent implements ControlValueAccessor {
  private static nextId = 0;
  private readonly generatedId = `lunex-input-${InputComponent.nextId++}`;

  label = input<string>('');
  type = input<InputType>('text');
  placeholder = input<string>('');
  id = input<string>('');
  name = input<string>('');
  required = input(false);
  disabled = model(false);
  readonly = input(false);
  error = input<string>('');
  hint = input<string>('');
  icon = input<IconName | ''>('');
  autocomplete = input<string>('off');

  // Password toggle
  showPassword = signal(false);
  showPasswordToggle = input(false);

  value = signal('');

  fieldId = computed(() => this.id() || this.generatedId);
  messageId = computed(() => {
    if (this.error()) return `${this.fieldId()}-error`;
    if (this.hint()) return `${this.fieldId()}-hint`;
    return null;
  });

  effectiveType = computed(() =>
    this.showPassword() && this.type() === 'password' ? 'text' : this.type()
  );

  private onChange = (value: string) => {};
  private onTouched = () => {};

  writeValue(value: string): void {
    this.value.set(value ?? '');
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  onInput(event: Event) {
    const target = event.target as HTMLInputElement;
    this.value.set(target.value);
    this.onChange(target.value);
  }

  onBlur() {
    this.onTouched();
  }

  onFocus() {
    // Focus handling if needed
  }

  togglePassword() {
    this.showPassword.update((v) => !v);
  }

  inputClasses = computed(() => {
    const base = 'input input-bordered w-full transition-all duration-200';
    const iconPadding = this.icon() ? 'pl-10' : '';
    const passwordPadding = this.showPasswordToggle() ? 'pr-10' : '';
    const errorState = this.error() ? 'input-error' : '';
    const disabledState = this.disabled() ? 'opacity-50 cursor-not-allowed' : '';
    
    return `${base} ${iconPadding} ${passwordPadding} ${errorState} ${disabledState}`;
  });
}