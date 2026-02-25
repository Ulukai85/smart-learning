import { provideHttpClientTesting } from '@angular/common/http/testing'
import { ActivatedRoute } from '@angular/router';

import { MessageService } from 'primeng/api';

export default [
  MessageService,
  provideHttpClientTesting(),
  {
    provide: ActivatedRoute,
    useValue: {},
  },
];
