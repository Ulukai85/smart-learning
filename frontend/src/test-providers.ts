import { ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';

export default [
  MessageService,
  {
    provide: ActivatedRoute,
    useValue: {},
  },
];
