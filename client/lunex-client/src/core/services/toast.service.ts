import { Injectable, signal, computed } from '@angular/core';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface Toast {
  id: string;
  type: ToastType;
  title: string;
  message?: string;
  duration?: number;
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private toasts = signal<Toast[]>([]);

  readonly allToasts = computed(() => this.toasts());

  show(toast: Omit<Toast, 'id'>) {
    const id = crypto.randomUUID();
    const newToast: Toast = { id, ...toast };
    this.toasts.update((current) => [...current, newToast]);

    if (toast.duration !== 0) {
      setTimeout(() => this.dismiss(id), toast.duration ?? 5000);
    }
  }

  success(title: string, message?: string) {
    this.show({ type: 'success', title, message, duration: 4000 });
  }

  error(title: string, message?: string) {
    this.show({ type: 'error', title, message, duration: 6000 });
  }

  warning(title: string, message?: string) {
    this.show({ type: 'warning', title, message, duration: 5000 });
  }

  info(title: string, message?: string) {
    this.show({ type: 'info', title, message, duration: 4000 });
  }

  dismiss(id: string) {
    this.toasts.update((current) => current.filter((t) => t.id !== id));
  }

  clear() {
    this.toasts.set([]);
  }
}