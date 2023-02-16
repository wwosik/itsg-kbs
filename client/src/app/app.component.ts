import { Component, OnInit } from '@angular/core';
import { LoggingService } from './common/logging.service';

@Component({
	selector: 'kbs-root',
	templateUrl: './app.component.html',
	styles: [],
})
export class AppComponent implements OnInit {
	ngOnInit(): void {
		this.log.info('started', { yay: true });
	}

	constructor(private readonly log: LoggingService) {}
}
