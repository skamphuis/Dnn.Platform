(function () {
    if (typeof window.dnn === 'undefined') window.dnn = {};
    if (typeof window.dnn.searchSkinObject === 'undefined') {
        window.dnn.searchSkinObject = function (options) {
            const defaultSettings = {
                delayTriggerAutoSearch: 100,
                minCharRequiredTriggerAutoSearch: 2,
                searchType: 'S',
                enableWildSearch: true,
                cultureCode: 'en-US'
            };
            this.settings = Object.assign({}, defaultSettings, options);
        };

        window.dnn.searchSkinObject.prototype = {
            _ignoreKeyCodes: [9, 13, 16, 17, 18, 19, 20, 27, 33, 34, 35, 36, 37, 38, 39, 40, 45],

            init: function () {
                let throttle = null;
                const self = this;

                const makeUrl = function (val, service) {
                    const url = service ? service.getServiceRoot('internalservices') + 'searchService/preview' : null;
                    if (!url) return null;

                    const params = new URLSearchParams();
                    params.append('keywords', val.trim());
                    if (!self.settings.enableWildSearch) params.append('forceWild', '0');
                    params.append('culture', self.settings.cultureCode);
                    if (self.settings.portalId >= 0) params.append('portal', self.settings.portalId);

                    return `${url}${url.includes('?') ? '&' : '?'}${params.toString()}`;
                };

                const generatePreviewTemplate = function (data, wrap) {
                    const existingPreview = wrap.querySelector('.searchSkinObjectPreview');
                    if (existingPreview) {
                        existingPreview.remove();
                    }

                    const ul = document.createElement('ul');
                    ul.className = 'searchSkinObjectPreview';

                    if (data && data.length) {
                        data.forEach(group => {
                            if (group.Results && group.Results.length) {
                                const groupLi = document.createElement('li');
                                groupLi.className = 'searchSkinObjectPreview_group';
                                groupLi.textContent = group.DocumentTypeName;
                                ul.appendChild(groupLi);

                                group.Results.forEach(item => {
                                    const li = document.createElement('li');
                                    li.dataset.url = item.DocumentUrl;

                                    if (item.Attributes.Avatar) {
                                        const span = document.createElement('span');
                                        const img = document.createElement('img');
                                        img.src = item.Attributes.Avatar;
                                        img.className = 'userpic';
                                        span.appendChild(img);
                                        li.appendChild(span);
                                    }

                                    const titleSpan = document.createElement('span');
                                    titleSpan.textContent = item.Title;
                                    li.appendChild(titleSpan);

                                    if (item.Description) {
                                        const descP = document.createElement('p');
                                        descP.textContent = item.Description;
                                        li.appendChild(descP);
                                    }

                                    if (item.Snippet) {
                                        const snippetP = document.createElement('p');
                                        snippetP.textContent = item.Snippet;
                                        li.appendChild(snippetP);
                                    }

                                    ul.appendChild(li);
                                });
                            }
                        });

                        const moreLi = document.createElement('li');
                        const moreLink = document.createElement('a');
                        moreLink.href = 'javascript:void(0)';
                        moreLink.className = 'searchSkinObjectPreview_more';
                        moreLink.textContent = wrap.getAttribute('data-moreresults');
                        moreLi.appendChild(moreLink);
                        ul.appendChild(moreLi);
                    } else {
                        const noResultLi = document.createElement('li');
                        noResultLi.textContent = wrap.getAttribute('data-noresult');
                        ul.appendChild(noResultLi);
                    }

                    wrap.appendChild(ul);

                    // Add click events
                    ul.querySelectorAll('li[data-url]').forEach(li => {
                        li.addEventListener('click', function () {
                            const navigateUrl = this.dataset.url;
                            if (navigateUrl) {
                                window.location.href = navigateUrl;
                            }
                            return false;
                        });
                    });

                    // Add more results click event
                    const moreLink = ul.querySelector('.searchSkinObjectPreview_more');
                    if (moreLink) {
                        moreLink.addEventListener('click', function (e) {
                            e.preventDefault();
                            let searchButton = wrap.nextElementSibling;
                            if (!searchButton) {
                                searchButton = wrap.parentElement.nextElementSibling;
                            }
                            if (searchButton) searchButton.click();
                        });
                    }
                };

                // Clear text button functionality
                document.querySelectorAll('.searchInputContainer a.dnnSearchBoxClearText').forEach(clearButton => {
                    clearButton.addEventListener('click', function (e) {
                        e.preventDefault();
                        const wrap = this.parentElement;
                        const input = wrap.querySelector('input');
                        input.value = '';
                        input.focus();
                        this.classList.remove('dnnShow');
                        const preview = wrap.querySelector('.searchSkinObjectPreview');
                        if (preview) preview.remove();
                    });
                });

                // Search button click handler
                document.querySelectorAll('.searchInputContainer').forEach(container => {
                    const nextElement = container.nextElementSibling;
                    if (nextElement) {
                        nextElement.addEventListener('click', function (e) {
                            const inputBox = this.previousElementSibling.querySelector('input[type="text"]');
                            if (!inputBox.value.length) {
                                e.preventDefault();
                            }
                        });
                    }
                });

                // Input handlers
                document.querySelectorAll('.searchInputContainer input').forEach(input => {
                    input.addEventListener('keyup', function (e) {
                        const k = e.keyCode || e.which;
                        if (self._ignoreKeyCodes.includes(k)) return;

                        const wrap = this.parentElement;
                        const val = this.value;
                        const container = wrap;

                        const clearButton = wrap.querySelector('a.dnnSearchBoxClearText');
                        const preview = wrap.querySelector('.searchSkinObjectPreview');

                        if (!val) {
                            if (clearButton) clearButton.classList.remove('dnnShow');
                            if (preview) preview.remove();
                        } else {
                            if (clearButton) clearButton.classList.add('dnnShow');

                            if (self.settings.searchType !== 'S' ||
                                val.length < self.settings.minCharRequiredTriggerAutoSearch) return;

                            if (throttle) {
                                clearTimeout(throttle);
                            }

                            throttle = setTimeout(async function () {
                                const service = window.dnnSF ? window.dnnSF(-1) : null;
                                const url = makeUrl(val, service);
                                if (url) {
                                    try {
                                        const response = await fetch(url, {
                                            headers: service ? service.setModuleHeaders() : {},
                                        });
                                        if (response.ok) {
                                            const result = await response.json();
                                            if (result) generatePreviewTemplate(result, container);
                                        }
                                    } catch (error) {
                                        console.error('Search request failed:', error);
                                    }
                                }
                            }, self.settings.delayTriggerAutoSearch);
                        }
                    });

                    input.addEventListener('paste', function () {
                        const event = new Event('keyup');
                        this.dispatchEvent(event);
                    });

                    input.addEventListener('keypress', function (e) {
                        const k = e.keyCode || e.which;
                        if (k === 13) {
                            e.preventDefault();
                            const val = this.value;
                            if (val.length) {
                                let searchButton = this.parentElement.nextElementSibling;
                                if (!searchButton) {
                                    searchButton = this.parentElement.parentElement.nextElementSibling;
                                }
                                if (searchButton) searchButton.click();
                            }
                        }
                    });
                });
            }
        };
    }
})();