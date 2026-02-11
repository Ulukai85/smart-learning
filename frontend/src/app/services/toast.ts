import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root',
})
export class Toast {
  private msgService = inject(MessageService);

  TOAST_KEY = 'global';

  success(summary: string, detail: string): void {
    this.showToast(summary, detail, 'success');
  }

  info(summary: string, detail: string): void {
    this.showToast(summary, detail, 'info');
  }

  warn(summary: string, detail: string): void {
    this.showToast(summary, detail, 'warn');
  }

  error(summary: string, detail: string): void {
    this.showToast(summary, detail, 'error');
  }

  private showToast(summary: string, detail: string, severity: string): void {
    console.log('msg service opened');
    this.msgService.add({
      key: this.TOAST_KEY,
      severity: severity,
      summary: summary,
      detail: detail,
      life: 3000,
      sticky: false,
    });
  }
}
