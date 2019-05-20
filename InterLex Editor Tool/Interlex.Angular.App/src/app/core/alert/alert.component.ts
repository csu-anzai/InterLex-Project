import {Component, OnDestroy, OnInit} from '@angular/core';
import {AlertService} from '../services/alert.service';
import {Subscription} from 'rxjs';
import {MessageService} from 'primeng/api';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.css'],
  providers: [MessageService]
})
export class AlertComponent implements OnInit, OnDestroy {

  private subscription: Subscription;

  constructor(private alertService: AlertService, private messageService: MessageService) {
  }

  ngOnInit() {
    this.subscription = this.alertService.getMessage().subscribe(message => {
      this.messageService.add({severity: message.type, detail: message.text});
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

}
