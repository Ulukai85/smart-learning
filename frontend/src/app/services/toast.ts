import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root',
})
export class Toast {
  private msgService = inject(MessageService);

  showToast(summary:string, detail:string, severity:string): void {
    console.log('msg service opened')
    this.msgService.add({
      key: 'global', 
      severity: severity, 
      summary: summary,
      detail: detail,
      life: 2000,
      sticky: true });
  }
}
